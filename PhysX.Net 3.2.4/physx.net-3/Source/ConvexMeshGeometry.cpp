#include "StdAfx.h"
#include <PxConvexMeshGeometry.h> 
#include "ConvexMeshGeometry.h"
#include "ConvexMesh.h"

ConvexMeshGeometry::ConvexMeshGeometry()
	: Geometry(GeometryType::ConvexMesh)
{
	
}
ConvexMeshGeometry::ConvexMeshGeometry(PhysX::ConvexMesh^ convexMesh, [Optional] Nullable<MeshScale> scale)
	: Geometry(GeometryType::ConvexMesh)
{
	this->Scale = scale.GetValueOrDefault(MeshScale::Identity);
	this->ConvexMesh = convexMesh;
}

PxGeometry* ConvexMeshGeometry::ToUnmanaged()
{
	PxConvexMeshGeometry* convexMesh = new PxConvexMeshGeometry();

	convexMesh->scale = MeshScale::ToUnmanaged(this->Scale);
	convexMesh->convexMesh = GetPointerOrNull(this->ConvexMesh);

	return convexMesh;
}
ConvexMeshGeometry^ ConvexMeshGeometry::ToManaged(PxConvexMeshGeometry convexMesh)
{
	ConvexMeshGeometry^ cm = gcnew ConvexMeshGeometry();

	cm->Scale = MeshScale::ToManaged(convexMesh.scale);
	cm->ConvexMesh = convexMesh.convexMesh == NULL ? 
		nullptr : 
		ObjectTable::GetObject<PhysX::ConvexMesh^>((intptr_t)convexMesh.convexMesh);

	return cm;
}