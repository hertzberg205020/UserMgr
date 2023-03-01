using MediatR;
using UserMgr.Domain.ValueObjects;

namespace UserMgr.Domain.Events;

public record UserAccessResultEvent(PhoneNumber PhoneNumber, UserAccessResult UserAccessResult): INotification;