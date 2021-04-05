using NUnit.Framework;

namespace JuanMartin.Kernel.Formatters.Tests
{
    [TestFixture]
    public class ValueHolderTests
    {
        [Test]
        public void AddAngGetAnnotation()
        {
            ValueHolder parent = new ValueHolder("Parent");

            parent.AddAnnotation("Child", "foo");

            ValueHolder child = parent.GetAnnotation("Child");

            Assert.AreEqual(child.Value, "foo");
        }

        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public void RemoveAnnotation()
        {
            ValueHolder parent = new ValueHolder("Parent");

            parent.AddAnnotation("Child", "foo");

            ValueHolder child = parent.GetAnnotation("Child");

            parent.RemoveAnnotation("Child");

            child = parent.GetAnnotation("Child");

            Assert.AreEqual(child, null);
        }

        [Test]
        public void AssignValue()
        {
            ValueHolder holder = new ValueHolder("int", 1);

            Assert.AreEqual(holder.Value, 1);
        }
    }
}
