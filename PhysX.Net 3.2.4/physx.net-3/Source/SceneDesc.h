#pragma once

#include <PxSceneDesc.h>
#include "SceneEnum.h"
#include "TolerancesScale.h"
#include "SimulationEventCallback.h"
#include <PxCpuDispatcher.h>
#include <PxTask.h>
#include <PxDefaultCpuDispatcher.h>

using namespace PhysX::Math;

namespace PhysX
{
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

		internal:
			property PxSceneDesc* UnmanagedPointer
			{
				PxSceneDesc* get();
			}
	};
};