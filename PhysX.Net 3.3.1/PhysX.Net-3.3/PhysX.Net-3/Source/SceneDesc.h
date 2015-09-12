#pragma once

#include "SceneEnum.h"
#include "TolerancesScale.h"
#include "SimulationEventCallback.h"

using namespace PhysX::Math;

namespace PhysX
{
	ref class ContactModifyCallback;
	ref class GpuDispatcher;

	public ref class SceneDesc : IDisposable
	{
		public:
			/// <summary>Raised before any disposing is performed.</summary>
			virtual event EventHandler^ OnDisposing;
			/// <summary>Raised once all disposing is performed.</summary>
			virtual event EventHandler^ OnDisposed;

		private:
			PxSceneDesc* _sceneDesc;

			SimulationEventCallback^ _simulationEventCallback;
			PhysX::ContactModifyCallback^ _contactModifyCallback;
			PhysX::GpuDispatcher^ _gpuDispatcher;

		public:
			SceneDesc([Optional] Nullable<PhysX::TolerancesScale> tolerancesScale);
			~SceneDesc();
		protected:
			!SceneDesc();
			
		public:
			property bool Disposed
			{
				virtual bool get();
			}

			bool IsValid();
			void SetToDefault([Optional] Nullable<TolerancesScale> tolerancesScale);

			/// <summary>
			/// Gravity vector.
			/// </summary>
			/// <remarks>When setting gravity, you should probably also set bounce threshold.</remarks>
			property Vector3 Gravity
			{
				Vector3 get();
				void set(Vector3 value);
			}

			/// <summary>
			/// Selects the friction algorithm to use for simulation.
			/// </summary>
			property PhysX::FrictionType FrictionType
			{
				PhysX::FrictionType get();
				void set(PhysX::FrictionType value);
			}

			/// <summary>
			/// Possible notification callback.
			/// This callback will be associated with the
			/// client PX_DEFAULT_CLIENT. Please use PxScene::setSimulationEventCallback() to
			/// register callbacks for other clients.
			/// Default: NULL
			/// </summary>
			property PhysX::SimulationEventCallback^ SimulationEventCallback
			{
				PhysX::SimulationEventCallback^ get();
				void set(PhysX::SimulationEventCallback^ value);
			}

			property PhysX::ContactModifyCallback^ ContactModifyCallback
			{
				PhysX::ContactModifyCallback^ get();
				void set(PhysX::ContactModifyCallback^ value);
			}

			property int CCDMaximumPasses
			{
				int get();
				void set(int value);
			}

			property PhysX::GpuDispatcher^ GpuDispatcher
			{
				PhysX::GpuDispatcher^ get();
				void set(PhysX::GpuDispatcher^ value);
			}

		internal:
			property PxSceneDesc* UnmanagedPointer
			{
				PxSceneDesc* get();
			}
	};
};