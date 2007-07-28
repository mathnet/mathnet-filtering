using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Packages.Standard
{
    public static class StdAspect
    {
        /// <summary>
        /// Flag indicating that the signal is constant (hence e.g. its derivative is zero).
        /// </summary>
        public static readonly NodeFlag ConstantFlag =
            NodeFlag.Register(new MathIdentifier("Constant", "Std"), typeof(StdAspect));

        /// <summary>
        /// Flag indicating that the signal is constrained to always have integer values.
        /// </summary>
        public static readonly NodeFlag IntegerConstraintFlag =
            NodeFlag.Register(new MathIdentifier("IntegerConstraint", "Std"), typeof(StdAspect), FlagKind.Constraint);

        /// <summary>
        /// Flag indicating that the signal is constrained to always have rational values.
        /// </summary>
        public static readonly NodeFlag RationalConstraintFlag =
            NodeFlag.Register(new MathIdentifier("RationalConstraint", "Std"), typeof(StdAspect), FlagKind.Constraint,
                new NodeEventTrigger(EventTriggerAction.Enable, IntegerConstraintFlag, IntegerConstraintFlag.FlagEnabledEvent),
                new NodeEventTrigger(EventTriggerAction.Dirty, IntegerConstraintFlag.FlagChangedEvent, IntegerConstraintFlag.FlagDirtiedEvent));

        /// <summary>
        /// Flag indicating that the signal is constrained to always have real values.
        /// </summary>
        public static readonly NodeFlag RealConstraintFlag =
            NodeFlag.Register(new MathIdentifier("RealConstraint", "Std"), typeof(StdAspect), FlagKind.Constraint,
                new NodeEventTrigger(EventTriggerAction.Enable, RationalConstraintFlag, RationalConstraintFlag.FlagEnabledEvent),
                new NodeEventTrigger(EventTriggerAction.Dirty, RationalConstraintFlag.FlagChangedEvent, RationalConstraintFlag.FlagDirtiedEvent));

        /// <summary>
        /// Flag indicating that the signal is constrained to always be >= 0.
        /// </summary>
        public static readonly NodeFlag PositiveOrZeroConstraintFlag =
            NodeFlag.Register(new MathIdentifier("PositiveOrZero", "Std"), typeof(StdAspect), FlagKind.Constraint);

        /// <summary>
        /// Flag indicating that the signal is constrained to always be > 0.
        /// </summary>
        public static readonly NodeFlag PositiveWithoutZeroConstraingFlag =
            NodeFlag.Register(new MathIdentifier("PositiveWithoutZero", "Std"), typeof(StdAspect), FlagKind.Constraint,
                new NodeEventTrigger(EventTriggerAction.Enable, PositiveOrZeroConstraintFlag, PositiveOrZeroConstraintFlag.FlagEnabledEvent),
                new NodeEventTrigger(EventTriggerAction.Dirty, PositiveOrZeroConstraintFlag.FlagChangedEvent));
    }
}
