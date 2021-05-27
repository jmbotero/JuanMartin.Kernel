using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Test.Adapters;
using NUnit.Framework;

namespace JuanMartin.Kernel.Adapters.Tests
{
    [TestFixture]
    public class AdapterMySqlTests
    {
        [Test]
        public static void ShouldRecieveFooReplyOnTetAdapterMessageRequest()
        {
            AdapterMySqlMock adapter = new AdapterMySqlMock();
            var actualResposeAnnotation = "name";
            var expectedRespoationseAnnotationValue = "foo";
                
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation

            request.AddData("uspAdapterTest");
            request.AddSender("MysqlTest", typeof(AdapterMySqlTests).ToString());

            adapter.Send(request);
            IRecordSet reply = (IRecordSet)adapter.Receive();

            Assert.AreNotEqual(reply.Data.Annotations.Count, 0, "has reply annotations");
            Assert.AreEqual(expectedRespoationseAnnotationValue, reply.Data.GetAnnotationByValue(1).GetAnnotation(actualResposeAnnotation).Value );
            Assert.AreEqual(1, reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);
        }
    }
}
