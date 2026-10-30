using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure;

public sealed class AccountService(DealershipDbContext db, UserManager<ApplicationUser> accounts,
    SignInManager<ApplicationUser> signIn, TokenIssuer tokens, UserService users) : IAccountService
{
    private static readonly ApplicationUser UnknownAccount = new();

    private static readonly PasswordHasher<ApplicationUser> UnknownHasher = new();
    private static readonly string UnknownHash = UnknownHasher.HashPassword(UnknownAccount, Guid.NewGuid().ToString());

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var email = request.Email.Trim();
        if (await accounts.FindByEmailAsync(email) != null)
        {
            throw new BusinessException(FailureKind.Conflict, "An account with this email already exists.");
        }

        var profile = new User { Name = Rules.Text(request.Name), Role = UserRole.Customer };
        var account = new ApplicationUser { Id = profile.Id, Profile = profile, UserName = email, Email = email, LockoutEnabled = true };
        db.Add(profile);
        EnsureSuccess(await accounts.CreateAsync(account, request.Password));
        return tokens.Issue(account);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var account = await accounts.FindByEmailAsync(request.Email.Trim());
        if (account == null)
        {
            UnknownHasher.VerifyHashedPassword(UnknownAccount, UnknownHash, request.Password);
            throw new BusinessException(FailureKind.Unauthorized, "Invalid email or password.");
        }
        if (!(await signIn.CheckPasswordSignInAsync(account, request.Password, true)).Succeeded)
        {
            throw new BusinessException(FailureKind.Unauthorized, "Invalid email or password.");
        }

        await db.Entry(account).Reference(x => x.Profile).LoadAsync(ct);
        account.Profile.IsRemoved = false;
        EnsureSuccess(await accounts.UpdateSecurityStampAsync(account));
        return tokens.Issue(account);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken ct)
    {
        var account = Rules.Found(await accounts.FindByIdAsync(userId.ToString()));
        await users.CloseAsync(userId, ct);
        EnsureSuccess(await accounts.UpdateSecurityStampAsync(account));
    }

    public async Task ProvisionAsync(Guid actorId, Guid userId, ProvisionAccountRequest request, CancellationToken ct)
    {
        await users.RequireAsync(actorId, ct, UserRole.SystemAdmin);
        var profile = Rules.Found(await db.Set<User>().SingleOrDefaultAsync(x => x.Id == userId, ct));
        if (profile.Role == UserRole.SystemAdmin)
        {
            throw new BusinessException(FailureKind.Forbidden, "Administrators are provisioned through trusted configuration.");
        }

        if (await accounts.FindByIdAsync(userId.ToString()) != null)
        {
            throw new BusinessException(FailureKind.Conflict, "This user already has an account.");
        }

        var email = request.Email.Trim();
        EnsureSuccess(await accounts.CreateAsync(new ApplicationUser
        {
            Id = profile.Id,
            Profile = profile,
            Email = email,
            UserName = email,
            LockoutEnabled = true
        }, request.Password));
    }

    public async Task SetRoleAsync(Guid actorId, Guid userId, string role, CancellationToken ct)
    {
        await users.RequireAsync(actorId, ct, UserRole.SystemAdmin);
        var parsed = Rules.Parse<UserRole>(role);
        var account = Rules.Found(await accounts.FindByIdAsync(userId.ToString()));
        await db.Entry(account).Reference(x => x.Profile).LoadAsync(ct);
        if (parsed == UserRole.SystemAdmin || account.Profile.Role == UserRole.SystemAdmin)
        {
            throw new BusinessException(FailureKind.Forbidden, "System administrators are provisioned through trusted configuration.");
        }

        account.Profile.Role = parsed;
        EnsureSuccess(await accounts.UpdateSecurityStampAsync(account));
    }

    public async Task<UserDto> GetAsync(Guid userId, CancellationToken ct)
    {
        var user = Rules.Found(await db.Set<User>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId && !x.IsRemoved, ct));
        return new(user.Id, user.Name, user.Role.ToString());
    }

    internal static void EnsureSuccess(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new BusinessException(result.Errors.Any(x => x.Code is "DuplicateEmail" or "DuplicateUserName" or "ConcurrencyFailure") ? FailureKind.Conflict : FailureKind.Validation,
                "The account could not be updated. Check the supplied values and password requirements.");
        }
    }
}
