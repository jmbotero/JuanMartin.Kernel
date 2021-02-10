namespace JuanMartin.Kernel.Listeners
{
    public class Scheduler
    {
    }
}

/*
I was thinking that perhaps I could create the objects at runtime and
use an interface for the known method to then call it:
>
object o = Activator.CreateInstance(type);
IPersist persist = o as IPersist; //IPersist is same sig as PersistXml
if (persist != null)
{
persist.Persist(XmlData);
}
That's exactly the way to do it.
*/
