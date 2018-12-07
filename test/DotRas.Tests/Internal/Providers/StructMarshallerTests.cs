﻿using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using DotRas.Internal.Providers;
using DotRas.Tests.Internal.Stubs;

namespace DotRas.Tests.Internal.Providers
{
    [TestFixture]
    public class StructMarshallerTests
    {
        [Test]
        public void IdentifyTheCorrectSizeOfAnInteger()
        {
            var target = new StructMarshaller();
            var result = target.SizeOf<int>();

            Assert.AreEqual(4, result);
        }

        [Test]
        public void IdentifyTheCorrectSizeOfAStructure()
        {
            var target = new StructMarshaller();
            var result = target.SizeOf<StubStructure>();

            Assert.AreEqual(32, result);
        }

        [Test]
        public void FreeTheMemoryAllocated()
        {
            var result = IntPtr.Zero;
            var target = new StubStructMarshaller();

            try
            {
                result = Marshal.AllocHGlobal(4);
                Assert.AreNotEqual(IntPtr.Zero, result);
            }
            finally
            {
                target.FreeHGlobalIfNeeded(result);

                Assert.IsTrue(target.ReleasedUnmanagedMemory);
            }
        }

        [Test]
        public void NotFreeTheMemoryAllocatedIfPtrIsZero()
        {
            var target = new StubStructMarshaller();
            target.FreeHGlobalIfNeeded(IntPtr.Zero);

            Assert.IsFalse(target.ReleasedUnmanagedMemory);
        }

        [Test]
        public void AllocateThePointerSpecified()
        {
            var result = IntPtr.Zero;

            try
            {
                var target = new StructMarshaller();
                result = target.AllocHGlobal(4);
                
                Assert.AreNotEqual(IntPtr.Zero, result);
            }
            finally 
            {
                if (result != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(result);
                }
            }
        }

        [Test]
        public void MarshalTheValueToAPointer()
        {
            var lpStubStructure = IntPtr.Zero;
            var target = new StructMarshaller();

            try
            {
                var sizeOfStubStructure = Marshal.SizeOf<StubStructure>();
                var value = new StubStructure
                {
                    Field1 = 1,
                    Field2 = 2,
                    Field3 = 3,
                    Field4 = "4"
                };

                lpStubStructure = Marshal.AllocHGlobal(sizeOfStubStructure);
                target.StructureToPtr(value, lpStubStructure);

                var result = Marshal.PtrToStructure<StubStructure>(lpStubStructure);

                Assert.AreEqual(1, result.Field1);
                Assert.AreEqual(2, result.Field2);
                Assert.AreEqual(3, result.Field3);
                Assert.AreEqual("4", result.Field4);
            }
            finally
            {
                if (lpStubStructure != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpStubStructure);
                }
            }
        }

        [Test]
        public void ThrowAnExceptionWhenThePtrIsZero()
        {
            var target = new StructMarshaller();
            Assert.Throws<ArgumentNullException>(() => target.StructureToPtr(new StubStructure(), IntPtr.Zero));
        }

        [Test]
        public void ThrowsAnExceptionWhenTheSizeIsZero()
        {
            var target = new StructMarshaller();
            Assert.Throws<ArgumentException>(() => target.AllocHGlobal(0));
        }

        [Test]
        public void ThrowsAnExceptionWhenTheSizeIsLessThanZero()
        {
            var target = new StructMarshaller();
            Assert.Throws<ArgumentException>(() => target.AllocHGlobal(0));
        }
    }
}