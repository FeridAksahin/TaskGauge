using TaskGauge.DataTransferObject;
using TaskGauge.ViewModel;

public class RoomStatic
{

    private static RoomStatic instance;


    private RoomStatic()
    {

    }


    public static RoomStatic Instance
    {
        get
        {

            if (instance == null)
            {
                instance = new RoomStatic();
            }

            return instance;
        }
    }

    public List<RoomUserDto> roomUser = new List<RoomUserDto>();
    public List<Room> room = new List<Room>();
    public List<TaskDto> allRoomTask = new List<TaskDto>();
    public List<TaskEffortViewModel> taskEffortList = new List<TaskEffortViewModel>();
}
