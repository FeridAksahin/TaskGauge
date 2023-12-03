using TaskGauge.DataTransferObject;

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
}
