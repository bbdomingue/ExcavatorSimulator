#pragma once

#include "Enums.h"
#include "Groups Mask.h"
#include "Description.h"

using namespace System;
namespace StillDesign
{
	namespace PhysX
	{
		ref class Material;
		ref class Scene;
		ref class CCDSkeleton;
		
		public ref class ShapeDescription abstract : Description
		{
			protected:
				NxShapeDesc* _desc;
				
				StillDesign::PhysX::Material^ _material;
				StillDesign::PhysX::CCDSkeleton^ _ccdSkeleton;
			
			protected:
				ShapeDescription( NxShapeDesc* description );
			public:
				~ShapeDescription();
			protected:
				!ShapeDescription();
				
			public:
				/// <summary>Is the shape description valid</summary>
				virtual bool IsValid();
				/// <summary>Returns 0 if the current settings are valid.</summary>
				virtual int CheckValid() override;
				/// <summary>Resets the description to default</summary>
				virtual void SetToDefault();
				
				/// <summary>Gets or Sets the type of shape. This value is specified during construction</summary>
				property ShapeType Type
				{
					ShapeType get();
				}
				
				/// <summary>Gets or Sets the name of the shape</summary>
				property String^ Name
				{
					String^ get();
					void set( String^ value );
				}
				
				property Matrix LocalPose
				{
					Matrix get();
					void set( Matrix value );
				}
				property Vector3 LocalPosition
				{
					Vector3 get();
					void set( Vector3 value );
				}
				property Matrix LocalRotation
				{
					Matrix get();
					void set( Matrix value );
				}
				
				/// <summary>Gets or Sets the Material of the Shape</summary>
				property StillDesign::PhysX::Material^ Material
				{
					StillDesign::PhysX::Material^ get();
					void set( StillDesign::PhysX::Material^ value );
				}
				/// <summary>Gets or Sets the Continuous Collision Detection Skeleton</summary>
				property StillDesign::PhysX::CCDSkeleton^ CCDSkeleton
				{
					StillDesign::PhysX::CCDSkeleton^ get();
					void set( StillDesign::PhysX::CCDSkeleton^ value );
				}
				
				/// <summary>Gets or Sets the Density of the Shape</summary>
				property float Density
				{
					float get();
					void set( float value );
				}
				/// <summary>Gets or Sets the Mass of the Shape</summary>
				property float Mass
				{
					float get();
					void set( float value );
				}
				property float SkinWidth
				{
					float get();
					void set( float value );
				}
				property StillDesign::PhysX::GroupsMask GroupsMask
				{
					StillDesign::PhysX::GroupsMask get();
					void set( StillDesign::PhysX::GroupsMask value );
				}
				
				property StillDesign::PhysX::ShapeFlag Flags
				{
					StillDesign::PhysX::ShapeFlag get();
					void set( StillDesign::PhysX::ShapeFlag value );
				}
				
				property ShapeCompartmentType NonInteractingCompartmentTypes
				{
					ShapeCompartmentType get();
					void set( ShapeCompartmentType value );
				}
				
				/// <summary>Gets or Sets an associative object.</summary>
				/// <remarks>This object is assigned to the Shape object when it is created.</remarks>
				property Object^ UserData;

			internal:
				property NxShapeDesc* UnmanagedPointer
				{
					NxShapeDesc* get();
				}
		};
	};
};