#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) IT大师
* CLR 版本: 4.0.30319.18408
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 官方网站：www.ITdos.com
* 创建日期：2015/5/10 10:54:32
* 文件描述：
******************************************************
* 修 改 人：ITdos
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Diagnostics;
using System.Collections;

namespace Dos.ORM
{
    public sealed class Check
    {
        #region Const Literals

        private const string DBC_CHECK_ALL = "DBC_CHECK_ALL";
        private const string DBC_CHECK_INVARIANT = "DBC_CHECK_INVARIANT";
        private const string DBC_CHECK_POSTCONDITION = "DBC_CHECK_POSTCONDITION";
        private const string DBC_CHECK_PRECONDITION = "DBC_CHECK_PRECONDITION";

        private const string PRECONDITION_COLON = "Precondition: ";
        private const string POSTCONDITION_COLON = "Postcondition: ";
        private const string ASSERTION_COLON = "Assertion: ";
        private const string INVARIANT_COLON = "Invariant: ";

        private const string PRECONDITION_FALIED = "Precondition failed.";
        private const string POSTCONDITION_FALIED = "Postcondition failed.";
        private const string ASSERTION_FAILED = "Assertion failed.";
        private const string INVARIANT_FALIED = "Invariant failed.";

        private const string NOT_NULL_FAILING_MESSAGE = "{0} could not be null";
        private const string NOT_NULL_OR_EMPTY_FAILING_MESSAGE = "{0} could not be null or empty";
        private const string IS_ASSIGNABLE_TO_FAILING_MESSAGE = "{0} is not assignable to {1}";
        private const string GREATER_THAN_FAILING_MESSAGE = "{0} must be > {1}";
        private const string GREATER_THAN_OR_EQUAL_FAILING_MESSAGE = "{0} must be >= {1}";
        private const string LESS_THAN_FAILING_MESSAGE = "{0} must be < {1}";
        private const string LESS_THAN_OR_EQUAL_FAILING_MESSAGE = "{0} must be <= {1}";

        #endregion  // End Const Literals

        #region Pluginable Check Strategies

        public interface ICheckStrategy
        {
            bool Pass(object obj);
            string GetFailingMessage(string objName);
        }

        private static void CheckByStrategies(object obj, string objName, ICheckStrategy[] strategies, ref bool assertion, ref string message)
        {
            if (strategies == null || strategies.Length == 0)
            {
                if (!NotNull.Pass(obj))
                {
                    assertion = false;
                    message = NotNull.GetFailingMessage(objName);
                }
            }
            else
            {
                for (int i = 0; i < strategies.Length; ++i)
                {
                    if (!strategies[i].Pass(obj))
                    {
                        assertion = false;
                        message = strategies[i].GetFailingMessage(objName);
                        break;
                    }
                }
            }
        }

        #region Predefined Check Strategies

        public static readonly ICheckStrategy NotNull = new NotNullCheckStrategy();
        public static readonly ICheckStrategy NotNullOrEmpty = new NotNullOrEmptyStrategy();
        public static ICheckStrategy IsAssignableTo<TargetType>()
        {
            return new IsAssignableToStrategy<TargetType>();
        }
        public static ICheckStrategy GreaterThan<T>(T compareValue)
        {
            return new GreaterThanStrategy<T>(compareValue);
        }
        public static ICheckStrategy LessThan<T>(T compareValue)
        {
            return new LessThanStrategy<T>(compareValue);
        }
        public static ICheckStrategy GreaterThanOrEqual<T>(T compareValue)
        {
            return new GreaterThanOrEqualStrategy<T>(compareValue);
        }
        public static ICheckStrategy LessThanOrEqual<T>(T compareValue)
        {
            return new LessThanOrEqualStrategy<T>(compareValue);
        }

        private sealed class NotNullCheckStrategy : ICheckStrategy
        {
            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                return obj != null;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(NOT_NULL_FAILING_MESSAGE, objName);
            }

            #endregion
        }

        private sealed class NotNullOrEmptyStrategy : ICheckStrategy
        {
            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj == null)
                    return false;
                else if (obj is string)
                    return !string.IsNullOrEmpty(obj as string);
                else if (obj is Array)
                    return (obj as Array).Length > 0;
                else if (obj is ICollection)
                    return (obj as ICollection).Count > 0;

                return true;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(NOT_NULL_OR_EMPTY_FAILING_MESSAGE, objName);
            }

            #endregion
        }

        private sealed class IsAssignableToStrategy<TargetType> : ICheckStrategy
        {
            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                return obj is TargetType;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(IS_ASSIGNABLE_TO_FAILING_MESSAGE, objName, typeof(TargetType));
            }

            #endregion
        }

        private sealed class GreaterThanStrategy<T> : ICheckStrategy
        {
            private T compareValue;

            public GreaterThanStrategy(T compareValue)
            {
                this.compareValue = compareValue;
            }

            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj is T && ((IComparable)obj).CompareTo(compareValue) > 0)
                    return true;

                return false;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(GREATER_THAN_FAILING_MESSAGE, objName, compareValue);
            }

            #endregion
        }

        private sealed class GreaterThanOrEqualStrategy<T> : ICheckStrategy
        {
            private T compareValue;

            public GreaterThanOrEqualStrategy(T compareValue)
            {
                this.compareValue = compareValue;
            }

            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj is T && ((IComparable)obj).CompareTo(compareValue) >= 0)
                    return true;

                return false;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(GREATER_THAN_OR_EQUAL_FAILING_MESSAGE, objName, compareValue);
            }

            #endregion
        }

        private sealed class LessThanStrategy<T> : ICheckStrategy
        {
            private T compareValue;

            public LessThanStrategy(T compareValue)
            {
                this.compareValue = compareValue;
            }

            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj is T && ((IComparable)obj).CompareTo(compareValue) < 0)
                    return true;

                return false;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(LESS_THAN_FAILING_MESSAGE, objName, compareValue);
            }

            #endregion
        }

        private sealed class LessThanOrEqualStrategy<T> : ICheckStrategy
        {
            private T compareValue;

            public LessThanOrEqualStrategy(T compareValue)
            {
                this.compareValue = compareValue;
            }

            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj is T && ((IComparable)obj).CompareTo(compareValue) <= 0)
                    return true;

                return false;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(LESS_THAN_OR_EQUAL_FAILING_MESSAGE, objName, compareValue);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Check Methods

        #region Precondition

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        [Conditional(DBC_CHECK_PRECONDITION)]
        public static void Require(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, PRECONDITION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, PRECONDITION_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        [Conditional(DBC_CHECK_PRECONDITION)]
        public static void Require(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException(message, inner);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, PRECONDITION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, PRECONDITION_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        [Conditional(DBC_CHECK_PRECONDITION)]
        public static void Require(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException(PRECONDITION_FALIED);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, PRECONDITION_FALIED);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, PRECONDITION_FALIED);
#endif
            }
        }

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        [Conditional(DBC_CHECK_PRECONDITION)]
        public static void Require(object obj, string objName, params ICheckStrategy[] strategies)
        {
            bool assertion = true;
            string message = null;

            CheckByStrategies(obj, objName, strategies, ref assertion, ref message);

            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, PRECONDITION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, PRECONDITION_COLON + message);
#endif
            }
        }

        #endregion  // End Precondition

        #region Postcondition

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        public static void Ensure(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, POSTCONDITION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, POSTCONDITION_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        public static void Ensure(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException(message, inner);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, POSTCONDITION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, POSTCONDITION_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        public static void Ensure(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException(POSTCONDITION_FALIED);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, POSTCONDITION_FALIED);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, POSTCONDITION_FALIED);
#endif
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        [Conditional(DBC_CHECK_POSTCONDITION)]
        public static void Ensure(object obj, string objName, params ICheckStrategy[] strategies)
        {
            bool assertion = true;
            string message = null;

            CheckByStrategies(obj, objName, strategies, ref assertion, ref message);

            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, POSTCONDITION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, POSTCONDITION_COLON + message);
#endif
            }
        }

        #endregion  // End Postcondition

        #region Invariant

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        public static void Invariant(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, INVARIANT_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, INVARIANT_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        public static void Invariant(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException(message, inner);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, INVARIANT_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, INVARIANT_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        public static void Invariant(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException(INVARIANT_FALIED);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, INVARIANT_FALIED);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, INVARIANT_FALIED);
#endif
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        [Conditional(DBC_CHECK_INVARIANT)]
        public static void Invariant(object obj, string objName, params ICheckStrategy[] strategies)
        {
            bool assertion = true;
            string message = null;

            CheckByStrategies(obj, objName, strategies, ref assertion, ref message);

            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, INVARIANT_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, INVARIANT_COLON + message);
#endif
            }
        }

        #endregion  // End Invariant

        #region Assertion

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        public static void Assert(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, ASSERTION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, ASSERTION_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        public static void Assert(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(message, inner);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, ASSERTION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, ASSERTION_COLON + message);
#endif
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        public static void Assert(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(ASSERTION_FAILED);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, ASSERTION_FAILED);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, ASSERTION_FAILED);
#endif
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional(DBC_CHECK_ALL)]
        public static void Assert(object obj, string objName, params ICheckStrategy[] strategies)
        {
            bool assertion = true;
            string message = null;

            CheckByStrategies(obj, objName, strategies, ref assertion, ref message);

            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(message);
            }
            else
            {
#if USE_TRACE_ASSERTION
                Trace.Assert(assertion, ASSERTION_COLON + message);
#elif USE_DEBUG_ASSERTION
                Debug.Assert(assertion, ASSERTION_COLON + message);
#endif
            }
        }

        #endregion  // End Assertion

        #endregion // Check Methods

        #region Use Exception Or Trace/Debug Assertion?

        // No creation
        private Check() { }

        /// <summary>
        /// Is exception handling being used?
        /// </summary>
        private static bool UseExceptions
        {
            get
            {
                return !useAssertions;
            }
            set
            {
                useAssertions = !value;
            }
        }

        // Are trace assertion statements being used? 
        // Default is to use exception handling.
#if USE_TRACE_ASSERTION || USE_DEBUG_ASSERTION
        private static bool useAssertions = true;
#else
        private static bool useAssertions = false;
#endif
        #endregion // End Use Exception Or Trace/Debug Assertion?

    }
    #region Exceptions

    /// <summary>
    /// Exception raised when a contract is broken.
    /// Catch this exception type if you wish to differentiate between 
    /// any DesignByContract exception and other runtime exceptions.
    ///  
    /// </summary>
    [Serializable]
    public abstract class DesignByContractException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignByContractException"/> class.
        /// </summary>
        protected DesignByContractException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignByContractException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected DesignByContractException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignByContractException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        protected DesignByContractException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception raised when a precondition fails.
    /// </summary>
    [Serializable]
    public sealed class PreconditionException : DesignByContractException
    {
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException() { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message) : base(message) { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception raised when a postcondition fails.
    /// </summary>
    [Serializable]
    public sealed class PostconditionException : DesignByContractException
    {
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException() { }
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message) : base(message) { }
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception raised when an invariant fails.
    /// </summary>
    [Serializable]
    public sealed class InvariantException : DesignByContractException
    {
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException() { }
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message) : base(message) { }
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception raised when an assertion fails.
    /// </summary>
    [Serializable]
    public sealed class AssertionException : DesignByContractException
    {
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException() { }
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message) : base(message) { }
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message, Exception inner) : base(message, inner) { }
    }

    #endregion // Exception classes
}
