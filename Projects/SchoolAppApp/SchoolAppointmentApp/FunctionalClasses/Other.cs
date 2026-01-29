namespace SchoolAppointmentApp.FunctionalClasses;

public static class ToArray
{
    public static bool[] ToBooleanArray(int sizeOfArray, int[] indiciesToTrue)
    {
        bool[] boolArray = new bool[sizeOfArray];
        foreach (int index in indiciesToTrue)
        {
            boolArray[index] = true;
        }
        return boolArray;
    }
}
