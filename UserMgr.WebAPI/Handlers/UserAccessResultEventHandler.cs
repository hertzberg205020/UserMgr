using MediatR;
using UserMgr.Domain;
using UserMgr.Domain.Events;
using UserMgr.Infrastructure;

namespace UserMgr.WebAPI.Handlers;

public class UserAccessResultEventHandler: INotificationHandler<UserAccessResultEvent>
{
    // private readonly IUserRepository _userRepository;
    // private readonly UserDbContext _userDbContext;
    //
    // public UserAccessResultEventHandler(IUserRepository userRepository, UserDbContext userDbContext)
    // {
    //     _userRepository = userRepository;
    //     _userDbContext = userDbContext;
    // }
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserAccessResultEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(UserAccessResultEvent notification, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            UserDbContext userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            await userRepository.AddNewLoginHistoryAsync(notification.PhoneNumber,
                $"登入結果是{notification.UserAccessResult}");
            await userDbContext.SaveChangesAsync(cancellationToken);
        }
        
    }
}