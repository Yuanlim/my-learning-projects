using SchoolAppointmentApp.DataTypeObject;
using SchoolAppointmentApp.Entities;

namespace SchoolAppointmentApp.Mapping;

public static class CommomMapping
{
    public static BlockDto ToBlockDto(this Block block)
    {
        return new
        (
            InitiatorId: block.InitiatorId,
            ReceiverId: block.ReceiverId,
            Blocked: block.Blocked
        );
    }

}
