public class Action
{
    public int action_id {get; private set;} = 0;
    public Action(int action_id = 0)
    {
        this.action_id = action_id;
    }

    public bool IsUp()
    {
        return action_id == 1;
    }

    public bool IsRight()
    {
        return action_id == 2;
    }

    public bool IsDown()
    {
        return action_id == 3;
    }

    public bool IsLeft()
    {
        return action_id == 4;
    }

    public void SetUp()
    {
        action_id = 1;
    }
    public void SetRight()
    {
        action_id = 2;
    }
    public void SetDown()
    {
        action_id = 3;
    }
    public void SetLeft()
    {
        action_id = 4;
    }

    public void Reverse()
    {
        switch (action_id)
        {
            case 1:
                action_id = 3;
                break;
            case 2:
                action_id = 4;
                break;
            case 3:
                action_id = 1;
                break;
            case 4:
                action_id = 2;
                break;
            default:
                throw new System.Exception("Invalid action");
        }
    }

    public override string ToString()
    {
        switch (action_id)
        {
            case 1:
                return "Up";
            case 2:
                return "Right";
            case 3:
                return "Down";
            case 4:
                return "Left";
            default:
                return "Invalid action";
        }
    }
}
