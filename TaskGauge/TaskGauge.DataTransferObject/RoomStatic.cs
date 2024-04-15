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

    public List<TaskEffortViewModel> taskEffortList = new List<TaskEffortViewModel>();
    public List<TotalTaskEffortInformationViewModel> totalTaskEffortInformation = new List<TotalTaskEffortInformationViewModel>();
}
