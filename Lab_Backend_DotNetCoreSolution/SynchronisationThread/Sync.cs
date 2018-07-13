using System;
using System.Collections.Generic;

public class Sync : ISync
{

    List<string> mylist = new List<string>();

    public Sync()
    {
        mylist.Add(DateTime.Now.TimeOfDay.ToString());
    }
    List<string> ISync.increment()
    {
        mylist.Add(DateTime.Now.TimeOfDay.ToString());

        return mylist;
    }
}