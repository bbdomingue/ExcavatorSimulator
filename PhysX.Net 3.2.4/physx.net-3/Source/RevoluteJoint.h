#pragma once

#include "Joint.h"
#include "JointEnum.h"

namespace PhysX
{
	ref class JointLimitPair;

	/// <summary>
	/// A joint which behaves in a similar way to a hinge or axle.
	/// </summary>
	public ref class RevoluteJoint : Joint
	{
		internal:
			RevoluteJoint(PxRevoluteJoint* joint, PhysX::Scene^ owner);

		public:
			/// <summary>
			/// Gets or sets the joint upper limit parameters.
			/// </summary>
			property JointLimitPair^ Limit
			{
				JointLimitPair^ get();
				void set(JointLimitPair^ value);
			}

			/// <summary>
			/// Gets or sets the target velocity for the drive model.
			/// </summary>
			property float DriveVelocity
			{
				float get();
				void set(float value);
			}

			/// <summary>
			/// Gets or sets the maximum torque the drive can exert.
			/// </summary>
			property float DriveForceLimit
			{
				float get();
				void set(float value);
			}

			/// <summary>
			/// Gets or sets the gear ratio for the drive.
			/// </summary>
			property float DriveGearRatio
			{
				float get();
				void set(float value);
			}

			/// <summary>
			/// Gets or sets flags specific to the Revolute Joint.
			/// </summary>
			property RevoluteJointFlag Flags
			{
				RevoluteJointFlag get();
				void set(RevoluteJointFlag value);
			}

			/// <summary>
			/// Gets or sets the linear tolerance threshold for projection.
			/// </summary>
			property float ProjectionLinearTolerance
			{
				float get();
				void set(float value);
			}

			/// <summary>
			/// Gets or sets the angular tolerance threshold for projection. Projection is enabled
			/// if ConstraintFlag.Projection is set for the joint.
			/// </summary>
			property float ProjectionAngularTolerance
			{
				float get();
				void set(float value);
			}

		internal:
			property PxRevoluteJoint* UnmanagedPointer
			{
				virtual PxRevoluteJoint* get() new;
			}
	};
};