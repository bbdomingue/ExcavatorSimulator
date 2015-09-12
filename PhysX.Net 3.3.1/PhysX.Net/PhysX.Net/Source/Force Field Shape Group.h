#pragma once


#include "Enums.h"
#include "Element Collection.h"
#include "Force Field Shape.h"

class NxForceFieldShapeGroup;

namespace StillDesign
{
	namespace PhysX
	{
		ref class ForceFieldShapeGroupDescription;
		ref class ForceFieldShapeDescription;
		ref class Scene;
		
		public ref class ForceFieldShapeGroup : StillDesign::PhysX::IDisposable
		{
			public:
				virtual event EventHandler^ OnDisposing;
				virtual event EventHandler^ OnDisposed;
			
			private:
				NxForceFieldShapeGroup* _group;
				
				StillDesign::PhysX::ForceField^ _forceField;
				
				StillDesign::PhysX::Scene^ _scene;
				
				ElementCollection< ForceFieldShape^ >^ _shapes;
				
				Object^ _userData;
				
			internal:
				ForceFieldShapeGroup( NxForceFieldShapeGroup* group );
			public:
				~ForceFieldShapeGroup();
			protected:
				!ForceFieldShapeGroup();
			public:
				property bool IsDisposed
				{
					virtual bool get();
				}
				
				ForceFieldShapeGroupDescription^ SaveToDescription();
				
				generic<class T>
				where T : ForceFieldShape
				T CreateShape( ForceFieldShapeDescription^ forceFieldShapeDescription );
				ForceFieldShape^ CreateShape( ForceFieldShapeDescription^ forceFieldShapeDescription );
				
				property StillDesign::PhysX::ForceField^ ForceField
				{
					StillDesign::PhysX::ForceField^ get();
					internal: void set( StillDesign::PhysX::ForceField^ value );
				}
				property StillDesign::PhysX::Scene^ Scene
				{
					StillDesign::PhysX::Scene^ get();
				}
				
				/// <summary></summary>
				property ReadOnlyList< ForceFieldShape^ >^ Shapes
				{
					ReadOnlyList< ForceFieldShape^ >^ get();
				}
				
				property String^ Name
				{
					String^ get();
					void set( String^ value );
				}
				
				/// <summary></summary>
				property ForceFieldShapeGroupFlag Flags
				{
					ForceFieldShapeGroupFlag get();
				}
				
				property Object^ UserData
				{
					Object^ get();
					void set( Object^ value );
				}
				
			internal:
				property NxForceFieldShapeGroup* UnmanagedPointer
				{
					NxForceFieldShapeGroup* get();
				}
		};
	};
};