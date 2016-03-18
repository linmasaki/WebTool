using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Package
{
    public Package()
    {
        ok = true;
    }
    public Package(string error)
    {
        ok = false;
        msg = error;
    }
    public bool ok { get; set; }
    public string msg { get; set; }
    public Dictionary<string, string> result { get; set; }
}
public class Package<T>
{
    public Package(T package)
    {
        ok = true;
        result = package;
    }

    public bool ok { get; set; }
    public string msg { get; set; }
    public T result { get; set; }
}