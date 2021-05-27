using NUnit.Framework;

namespace JuanMartin.Kernel.Formatters.Tests
{
    [TestFixture]
    public class ValueHolderTests
    {
        [Test]
        public void ShoulAddAnnotationToListOfAnnotationsInalueholderAsAnotherValueholder()
        {
            ValueHolder actualParent = new ValueHolder("Parent");
            var actualAnnotationName = "Child";
            var actualAnnotationValue = "foo";
            var expectedAnnotationCount = 1;
            var expectedAnnotationType = "JuanMartin.Kernel.ValueHolder";
            var expectedAnnotationName = "Child";
            var expectedAnnotationValue = "foo";

            actualParent.AddAnnotation(actualAnnotationName, actualAnnotationValue);

            Assert.AreEqual(expectedAnnotationCount, actualParent.Annotations.Count, "added to list of annotations");

            var actualChild = actualParent.GetAnnotation(actualAnnotationName);

            Assert.AreEqual(expectedAnnotationType, actualChild.GetType().ToString(), "annotation type");
            Assert.AreEqual(expectedAnnotationName, actualChild.Name, "annotation name");
            Assert.AreEqual(expectedAnnotationValue, actualChild.Value, "annotation value");
        }

        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        public void ShouldeleteAnnotationObjectOnRemoveAnnotation()
        {
            ValueHolder actualParent = new ValueHolder("Parent");

            actualParent.AddAnnotation("Child", "foo");

            ValueHolder actualChild = actualParent.GetAnnotation("Child");

            actualParent.RemoveAnnotation("Child");

            actualChild = actualParent.GetAnnotation("Child");

            Assert.AreEqual(null, actualChild);
        }
    }
}
