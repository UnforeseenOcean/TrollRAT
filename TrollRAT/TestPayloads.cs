using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TrollRAT
{
    public class TestPayload1 : Payload
    {
        public TestPayload1() { name = "Test 1"; }

        protected override void execute()
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }
    }

    public class TestPayload2 : LoopingPayload
    {
        public TestPayload2() { name = "Test 2"; }

        protected override void execute()
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }
    }
}
