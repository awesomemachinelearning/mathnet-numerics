// <copyright file="ResidualStopCriteriumTest.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Solvers;
using NUnit.Framework;

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Complex.Solvers.StopCriterium
{
#if NOSYSNUMERICS
    using Complex = Numerics.Complex;
#else
    using Complex = System.Numerics.Complex;
#endif

    /// <summary>
    /// Residual stop criterium tests.
    /// </summary>
    [TestFixture, Category("LASolver")]
    public sealed class ResidualStopCriteriumTest
    {
        /// <summary>
        /// Create with negative maximum throws <c>ArgumentOutOfRangeException</c>.
        /// </summary>
        [Test]
        public void CreateWithNegativeMaximumThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => new ResidualStopCriterium<Complex>(-0.1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Create with illegal minimum iterations throws <c>ArgumentOutOfRangeException</c>.
        /// </summary>
        [Test]
        public void CreateWithIllegalMinimumIterationsThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => new ResidualStopCriterium<Complex>(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Can create.
        /// </summary>
        [Test]
        public void Create()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-8, 50);

            Assert.AreEqual(1e-8, criterium.Maximum, "Incorrect maximum");
            Assert.AreEqual(50, criterium.MinimumIterationsBelowMaximum, "Incorrect iteration count");
        }

        /// <summary>
        /// Determine status with illegal iteration number throws <c>ArgumentOutOfRangeException</c>.
        /// </summary>
        [Test]
        public void DetermineStatusWithIllegalIterationNumberThrowsArgumentOutOfRangeException()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-8, 50);

            Assert.That(() => criterium.DetermineStatus(
                -1,
                Vector<Complex>.Build.Dense(3, 4),
                Vector<Complex>.Build.Dense(3, 5),
                Vector<Complex>.Build.Dense(3, 6)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Determine status with non-matching solution vector throws <c>ArgumentException</c>.
        /// </summary>
        [Test]
        public void DetermineStatusWithNonMatchingSolutionVectorThrowsArgumentException()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-8, 50);

            Assert.That(() => criterium.DetermineStatus(
                1,
                Vector<Complex>.Build.Dense(4, 4),
                Vector<Complex>.Build.Dense(3, 4),
                Vector<Complex>.Build.Dense(3, 4)), Throws.ArgumentException);
        }

        /// <summary>
        /// Determine status with non-matching source vector throws <c>ArgumentException</c>.
        /// </summary>
        [Test]
        public void DetermineStatusWithNonMatchingSourceVectorThrowsArgumentException()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-8, 50);

            Assert.That(() => criterium.DetermineStatus(
                1,
                Vector<Complex>.Build.Dense(3, 4),
                Vector<Complex>.Build.Dense(4, 4),
                Vector<Complex>.Build.Dense(3, 4)), Throws.ArgumentException);
        }

        /// <summary>
        /// Determine status with non-matching residual vector throws <c>ArgumentException</c>.
        /// </summary>
        [Test]
        public void DetermineStatusWithNonMatchingResidualVectorThrowsArgumentException()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-8, 50);

            Assert.That(() => criterium.DetermineStatus(
                1,
                Vector<Complex>.Build.Dense(3, 4),
                Vector<Complex>.Build.Dense(3, 4),
                Vector<Complex>.Build.Dense(4, 4)), Throws.ArgumentException);
        }

        /// <summary>
        /// Can determine status with source NaN.
        /// </summary>
        [Test]
        public void DetermineStatusWithSourceNaN()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-3, 10);

            var solution = new DenseVector(new[] {new Complex(1.0, 1), new Complex(1.0, 1), new Complex(2.0, 1)});
            var source = new DenseVector(new[] {new Complex(1.0, 1), new Complex(1.0, 1), new Complex(double.NaN, 1)});
            var residual = new DenseVector(new[] {new Complex(1000.0, 1), new Complex(1000.0, 1), new Complex(2001.0, 1)});

            var status = criterium.DetermineStatus(5, solution, source, residual);
            Assert.AreEqual(IterationStatus.Diverged, status, "Should be diverged");
        }

        /// <summary>
        /// Can determine status with residual NaN.
        /// </summary>
        [Test]
        public void DetermineStatusWithResidualNaN()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-3, 10);

            var solution = new DenseVector(new[] {new Complex(1.0, 1), new Complex(1.0, 1), new Complex(2.0, 1)});
            var source = new DenseVector(new[] {new Complex(1.0, 1), new Complex(1.0, 1), new Complex(2.0, 1)});
            var residual = new DenseVector(new[] {new Complex(1000.0, 1), new Complex(double.NaN, 1), new Complex(2001.0, 1)});

            var status = criterium.DetermineStatus(5, solution, source, residual);
            Assert.AreEqual(IterationStatus.Diverged, status, "Should be diverged");
        }

        /// <summary>
        /// Can determine status with convergence at first iteration.
        /// </summary>
        /// <remarks>Bugfix: The unit tests for the BiCgStab solver run with a super simple matrix equation
        /// which converges at the first iteration. The default settings for the
        /// residual stop criterium should be able to handle this.
        /// </remarks>
        [Test]
        public void DetermineStatusWithConvergenceAtFirstIteration()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-12);

            var solution = new DenseVector(new[] {Complex.One, Complex.One, Complex.One});
            var source = new DenseVector(new[] {Complex.One, Complex.One, Complex.One});
            var residual = new DenseVector(new[] {Complex.Zero, Complex.Zero, Complex.Zero});

            var status = criterium.DetermineStatus(0, solution, source, residual);
            Assert.AreEqual(IterationStatus.Converged, status, "Should be done");
        }

        /// <summary>
        /// Can determine status.
        /// </summary>
        [Test]
        public void DetermineStatus()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-3, 10);

            // the solution vector isn't actually being used so ...
            var solution = new DenseVector(new[] {new Complex(double.NaN, double.NaN), new Complex(double.NaN, double.NaN), new Complex(double.NaN, double.NaN)});

            // Set the source values
            var source = new DenseVector(new[] {new Complex(1.000, 1), new Complex(1.000, 1), new Complex(2.001, 1)});

            // Set the residual values
            var residual = new DenseVector(new[] {new Complex(0.001, 0), new Complex(0.001, 0), new Complex(0.002, 0)});

            var status = criterium.DetermineStatus(5, solution, source, residual);
            Assert.AreEqual(IterationStatus.Continue, status, "Should still be running");

            var status2 = criterium.DetermineStatus(16, solution, source, residual);
            Assert.AreEqual(IterationStatus.Converged, status2, "Should be done");
        }

        /// <summary>
        /// Can reset calculation state.
        /// </summary>
        [Test]
        public void ResetCalculationState()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-3, 10);

            var solution = new DenseVector(new[] {new Complex(0.001, 1), new Complex(0.001, 1), new Complex(0.002, 1)});
            var source = new DenseVector(new[] {new Complex(0.001, 1), new Complex(0.001, 1), new Complex(0.002, 1)});
            var residual = new DenseVector(new[] {new Complex(1.000, 0), new Complex(1.000, 0), new Complex(2.001, 0)});

            var status = criterium.DetermineStatus(5, solution, source, residual);
            Assert.AreEqual(IterationStatus.Continue, status, "Should be running");

            criterium.Reset();
            Assert.AreEqual(IterationStatus.Continue, criterium.Status, "Should not have started");
        }

        /// <summary>
        /// Can clone stop criterium.
        /// </summary>
        [Test]
        public void Clone()
        {
            var criterium = new ResidualStopCriterium<Complex>(1e-3, 10);

            var clone = criterium.Clone();
            Assert.IsInstanceOf(typeof(ResidualStopCriterium<Complex>), clone, "Wrong criterium type");

            var clonedCriterium = clone as ResidualStopCriterium<Complex>;
            Assert.IsNotNull(clonedCriterium);

            // ReSharper disable PossibleNullReferenceException
            Assert.AreEqual(criterium.Maximum, clonedCriterium.Maximum, "Clone failed");
            Assert.AreEqual(criterium.MinimumIterationsBelowMaximum, clonedCriterium.MinimumIterationsBelowMaximum, "Clone failed");

            // ReSharper restore PossibleNullReferenceException
        }
    }
}
