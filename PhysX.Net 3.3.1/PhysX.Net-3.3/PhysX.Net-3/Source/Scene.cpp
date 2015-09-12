#include "StdAfx.h"
#include "Scene.h"
#include "SceneDesc.h"
#include "Bounds3.h"
#include "QueryHit.h"
#include "QueryFilterData.h"
#include "Geometry.h"
#include "Actor.h"
#include "ActiveTransform.h"
#include "Physics.h"
#include "ControllerManager.h"
#include "Shape.h"
#include "RaycastHit.h"
#include "Joint.h"
#include "D6Joint.h"
#include "DistanceJoint.h"
#include "FixedJoint.h"
#include "RigidActor.h"
#include "PrismaticJoint.h"
#include "RevoluteJoint.h"
#include "SphericalJoint.h"
#include "SimulationStatistics.h"
#include "Articulation.h"
#include "Aggregate.h"
#include "SceneSweepOperationObject.h"
#include "SweepHit.h"
#include "FailedToCreateObjectException.h"
#include "RenderBuffer.h"
#include "VolumeCache.h"
#include "InternalRaycastCallback.h"
#include "QueryFilterCallback.h"
#include "QueryCache.h"
#include "OverlapHit.h"
#include "InternalOverlapCallback.h"
#include "Collection.h"
#include "SceneLimits.h"
#include "ContactModifyCallback.h"

//#include <PxFixedJoint.h>
//#include <PxCollectionExt.h>

using namespace PhysX;

Scene::Scene(PxScene* scene, PhysX::Physics^ physics)
{
	if (scene == NULL)
		throw gcnew ArgumentNullException("scene");
	ThrowIfNullOrDisposed(physics, "physics");

	_scene = scene;
	_physics = physics;

	ObjectTable::Add((intptr_t)scene, this, physics);
}
Scene::~Scene()
{
	this->!Scene();
}
Scene::!Scene()
{
	OnDisposing(this, nullptr);

	if (Disposed)
		return;

	_scene->release();
	_scene = NULL;

	OnDisposed(this, nullptr);
}

bool Scene::Disposed::get()
{
	return _scene == NULL;
}

void Scene::SetFlag(SceneFlag flag, bool value)
{
	this->UnmanagedPointer->setFlag(ToUnmanagedEnum(PxSceneFlag, flag), value);
}

//

PhysX::Physics^ Scene::Physics::get()
{
	return _physics;
}

RenderBuffer^ Scene::GetRenderBuffer()
{
	const PxRenderBuffer& buffer = this->UnmanagedPointer->getRenderBuffer();

	auto b = RenderBuffer::ToManaged(buffer);

	return b;
}

SimulationStatistics^ Scene::GetSimulationStatistics()
{
	PxSimulationStatistics stats;
	_scene->getSimulationStatistics(stats);

	return SimulationStatistics::ToManaged(&stats);
}

void Scene::ShiftOrigin(Vector3 shift)
{
	_scene->shiftOrigin(UV(shift));
}

#pragma region Actors
IEnumerable<Actor^>^ Scene::Actors::get()
{
	return ObjectTable::GetObjectsOfOwnerAndType<Actor^>(this->Physics);
}
int Scene::GetNumberOfActors(ActorTypeSelectionFlag types)
{
	return _scene->getNbActors(ToUnmanagedEnum(PxActorTypeSelectionFlag, types));
}

array<ActiveTransform^>^ Scene::GetActiveTransforms([Optional] Nullable<int> clientId)
{
	int c = clientId.GetValueOrDefault(PX_DEFAULT_CLIENT);

	PxU32 n;
	const PxActiveTransform* t = _scene->getActiveTransforms(n, c);

	auto transforms = gcnew array<ActiveTransform^>(n);
	for (PxU32 i = 0; i < n; i++)
	{
		transforms[i] = ActiveTransform::ToManaged(t[i]);
	}

	return transforms;
}

void Scene::AddActor(Actor^ actor)
{
	ThrowIfNullOrDisposed(actor, "actor");

	_scene->addActor(*actor->UnmanagedPointer);
}
void Scene::RemoveActor(Actor^ actor)
{
	ThrowIfNullOrDisposed(actor, "actor");

	_scene->removeActor(*actor->UnmanagedPointer);
}

/// <summary>Gets the articulations.</summary>
IEnumerable<Articulation^>^ Scene::Articulations::get()
{
	return ObjectTable::GetObjectsOfOwnerAndType<Articulation^>(this);
}
#pragma endregion

#pragma region Joints
Joint^ Scene::CreateJoint(JointType type, RigidActor^ actor0, Matrix localFrame0, RigidActor^ actor1, Matrix localFrame1)
{
	PxPhysics* physics = this->Physics->UnmanagedPointer;

	PxRigidActor* a0 = GetPointerOrNull(actor0);
	PxRigidActor* a1 = GetPointerOrNull(actor1);

	PxTransform lf0 = MathUtil::MatrixToPxTransform(localFrame0);
	PxTransform lf1 = MathUtil::MatrixToPxTransform(localFrame1);

	Joint^ joint = nullptr;

	switch (type)
	{
		case JointType::D6:
		{
			auto d6 = PxD6JointCreate(*physics, a0, lf0, a1, lf1);

			auto d6Joint = joint = gcnew D6Joint(d6, this);
		}
		break;

		case JointType::Distance:
		{
			auto distance = PxDistanceJointCreate(*physics, a0, lf0, a1, lf1);

			auto distanceJoint = joint = gcnew DistanceJoint(distance, this);
		}
		break;

		case JointType::Fixed:
		{
			auto fixed = PxFixedJointCreate(*physics, a0, lf0, a1, lf1);

			auto fixedJoint = joint = gcnew FixedJoint(fixed, this);
		}
		break;

		case JointType::Prismatic:
		{
			auto primatic = PxPrismaticJointCreate(*physics, a0, lf0, a1, lf1);

			auto primaticJoint = joint = gcnew PrismaticJoint(primatic, this);
		}
		break;

		case JointType::Revolute:
		{
			auto revolute = PxRevoluteJointCreate(*physics, a0, lf0, a1, lf1);

			auto revoluteJoint = joint = gcnew RevoluteJoint(revolute, this);
		}
		break;

		case JointType::Spherical:
		{
			auto spherical = PxSphericalJointCreate(*physics, a0, lf0, a1, lf1);

			auto sphericalJoint = joint = gcnew SphericalJoint(spherical, this);
		}
		break;
	}

	if (joint == nullptr)
		throw gcnew ArgumentException(String::Format("Unsupported joint type {0}", type));

	return joint;
}

generic<typename T> where T : Joint
T Scene::CreateJoint(RigidActor^ actor0, Matrix localFrame0, RigidActor^ actor1, Matrix localFrame1)
{
	JointType type;

	if (T::typeid == D6Joint::typeid)
		type = JointType::D6;

	else if (T::typeid == DistanceJoint::typeid)
		type = JointType::Distance;

	else if (T::typeid == FixedJoint::typeid)
		type = JointType::Fixed;

	else if (T::typeid == PrismaticJoint::typeid)
		type = JointType::Prismatic;

	else if (T::typeid == RevoluteJoint::typeid)
		type = JointType::Revolute;

	else if (T::typeid == SphericalJoint::typeid)
		type = JointType::Spherical;

	else
		throw gcnew ArgumentException("Unsupported joint type");

	return (T)CreateJoint(type, actor0, localFrame0, actor1, localFrame1);
}

IEnumerable<Joint^>^ Scene::Joints::get()
{
	// Extend this object table method to support inheritance selection
	return ObjectTable::GetObjectsOfOwnerAndType<Joint^>(this);
}
#pragma endregion

#pragma region Grouping
ConstraintDominance Scene::GetDominanceGroupPair (Byte group1, Byte group2)
{
	return ConstraintDominance::ToManaged(_scene->getDominanceGroupPair(group1, group2));
}

void Scene::SetDominanceGroupPair(Byte group1, Byte group2, ConstraintDominance dominance)
{
	_scene->setDominanceGroupPair(group1, group2, ConstraintDominance::ToUnmanaged(dominance));
}
#pragma endregion

void Scene::Simulate(float elapsedTime)
{
	_scene->simulate(elapsedTime);
}
bool Scene::CheckResults([Optional] bool block)
{
	return _scene->checkResults(block);
}
bool Scene::FetchResults([Optional] bool block)
{
	return _scene->fetchResults(block);
}
void Scene::FlushSimulation([Optional] bool sendPendingReports)
{
	_scene->flush(sendPendingReports);
}

int Scene::SolverBatchSize::get()
{
	return _scene->getSolverBatchSize();
}
void Scene::SolverBatchSize::set(int value)
{
	_scene->setSolverBatchSize(value);
}

Vector3 Scene::Gravity::get()
{
	return MathUtil::PxVec3ToVector3(_scene->getGravity());
}
void Scene::Gravity::set(Vector3 value)
{
	_scene->setGravity(MathUtil::Vector3ToPxVec3(value));
}

#pragma region Visualization
float Scene::GetVisualizationParameter(VisualizationParameter param)
{
	return _scene->getVisualizationParameter((PxVisualizationParameter::Enum)(PxU32)param);
}
bool Scene::SetVisualizationParameter(VisualizationParameter param, float value)
{
	return _scene->setVisualizationParameter((PxVisualizationParameter::Enum)(PxU32)param, value);
}
bool Scene::SetVisualizationParameter(VisualizationParameter param, bool value)
{
	return SetVisualizationParameter(param, value ? 1.0f : 0.0f);
}

Bounds3 Scene::VisualizationCullingBox::get()
{
	return Bounds3::ToManaged(_scene->getVisualizationCullingBox());
}
void Scene::VisualizationCullingBox::set(Bounds3 value)
{
	_scene->setVisualizationCullingBox(Bounds3::ToUnmanaged(value));
}
#pragma endregion

bool Scene::Raycast(Vector3 origin, Vector3 direction, float distance, int maximumHits, Func<array<RaycastHit^>^, bool>^ hitCall, [Optional] HitFlag hitFlag, [Optional] Nullable<QueryFilterData> filterData, [Optional] QueryFilterCallback^ filterCallback, [Optional] QueryCache^ cache)
{
	if (maximumHits < 0)
		throw gcnew ArgumentOutOfRangeException("maximumHits");
	ThrowIfNull(hitCall, "hitCall");

	PxVec3 o = UV(origin);
	PxVec3 d = UV(direction);

	PxRaycastHit* hits;
	try
	{
		hits = new PxRaycastHit[maximumHits];
		InternalRaycastCallback hc(hits, maximumHits, hitCall);

		PxHitFlags f = ToUnmanagedEnum(PxHitFlag, hitFlag);

		PxQueryFilterData fd = (filterData.HasValue ? QueryFilterData::ToUnmanaged(filterData.Value) : PxQueryFilterData());

		UserQueryFilterCallback* qfcb = (filterCallback == nullptr ? NULL : &UserQueryFilterCallback(filterCallback));

		PxQueryCache* qc = (cache == nullptr ? NULL : &QueryCache::ToUnmanaged(cache));

		bool result = _scene->raycast(o, d, distance, hc, f, fd, qfcb, qc);

		return result;
	}
	finally
	{
		delete[] hits;
	}
}

bool Scene::Overlap(Geometry^ geometry, Matrix pose, int maximumOverlaps, Func<array<OverlapHit^>^, bool>^ hitCall, [Optional] Nullable<QueryFilterData> filterData, [Optional] QueryFilterCallback^ filterCallback)
{
	ThrowIfNull(geometry, "geometry");
	if (maximumOverlaps < 0)
		throw gcnew ArgumentOutOfRangeException("maximumOverlaps");
	ThrowIfNull(hitCall, "hitCall");

	PxGeometry* g = geometry->ToUnmanaged();
	PxTransform p = MathUtil::MatrixToPxTransform(pose);

	PxOverlapHit* overlaps;
	try
	{
		overlaps = new PxOverlapHit[maximumOverlaps];
		InternalOverlapCallback oc(overlaps, maximumOverlaps, hitCall);

		PxQueryFilterData fd = (filterData.HasValue ? QueryFilterData::ToUnmanaged(filterData.Value) : PxQueryFilterData());

		UserQueryFilterCallback* qfcb = (filterCallback == nullptr ? NULL : &UserQueryFilterCallback(filterCallback));

		bool result = _scene->overlap(*g, p, oc, fd, qfcb);

		return result;
	}
	finally
	{
		delete[] overlaps;
	}
}

#pragma region Character
ControllerManager^ Scene::CreateControllerManager()
{
	PxControllerManager* manager = PxCreateControllerManager(*_scene);

	if (manager == NULL)
		throw gcnew FailedToCreateObjectException("Failed to create controller manager");

	ControllerManager^ c = gcnew ControllerManager(manager, this);

	return c;
}

#pragma region Articulation and Aggregate
void Scene::AddArticulation(Articulation^ articulation)
{
	ThrowIfNullOrDisposed(articulation, "articulation");

	_scene->addArticulation(*articulation->UnmanagedPointer);
}

void Scene::RemoveArticulation(Articulation^ articulation)
{
	_scene->removeArticulation(*articulation->UnmanagedPointer);
}

void Scene::AddAggregate(Aggregate^ aggregate)
{
	_scene->addAggregate(*aggregate->UnmanagedPointer);
}

void Scene::RemoveAggregate(Aggregate^ aggregate)
{
	_scene->removeAggregate(*aggregate->UnmanagedPointer);
}

IEnumerable<Aggregate^>^ Scene::Aggregates::get()
{
	return ObjectTable::GetObjectsOfOwnerAndType<Aggregate^>(this);
}
#pragma endregion

#pragma region Collection
Collection^ Scene::CreateCollection()
{
	PxCollection* collection = PxCollectionExt::createCollection(*_scene);

	return gcnew Collection(collection, this);
}
#pragma endregion

SimulationEventCallback^ Scene::GetSimulationEventCallback(int clientId)
{
	PxSimulationEventCallback* c = _scene->getSimulationEventCallback(clientId);

	if (c == NULL)
		return nullptr;

	// Get the SimulationEventCallback instance wrapping the unmanaged class
	if (ObjectTable::Contains((intptr_t)c))
		return ObjectTable::GetObject<SimulationEventCallback^>((intptr_t)c);
	else
		return nullptr;
}
void Scene::SetSimulationEventCallback(SimulationEventCallback^ callback, int clientId)
{
	_scene->setSimulationEventCallback(callback == nullptr ? NULL : callback->UnmanagedPointer, clientId);
}

int Scene::CreateClient()
{
	return _scene->createClient();
}

VolumeCache^ Scene::CreateVolumeCache(int maxStaticShapes, int maxDynamicShapes)
{
	PxVolumeCache* vc = _scene->createVolumeCache(maxStaticShapes, maxDynamicShapes);

	return gcnew VolumeCache(vc);
}

int Scene::DynamicTreeRebuildRateHint::get()
{
	return _scene->getDynamicTreeRebuildRateHint();
}
void Scene::DynamicTreeRebuildRateHint::set(int value)
{
	_scene->setDynamicTreeRebuildRateHint(value);
}

int Scene::Timestamp::get()
{
	return _scene->getTimestamp();
}

PhysX::FrictionType Scene::FrictionType::get()
{
	return (PhysX::FrictionType)_scene->getFrictionType();
}
void Scene::FrictionType::set(PhysX::FrictionType value)
{
	_scene->setFrictionType(ToUnmanagedEnum(PxFrictionType, value));
}

SceneFlag Scene::Flags::get()
{
	return ToManagedEnum(SceneFlag, _scene->getFlags());
}

int Scene::SceneQueryStaticTimestamp::get()
{
	return _scene->getSceneQueryStaticTimestamp();
}

PhysX::SceneLimits^ Scene::SceneLimits::get()
{
	return PhysX::SceneLimits::ToManaged(_scene->getLimits());
}
void Scene::SceneLimits::set(PhysX::SceneLimits^ value)
{
	_scene->setLimits(PhysX::SceneLimits::ToUnmanaged(value));
}

PhysX::ContactModifyCallback^ Scene::ContactModifyCallback::get()
{
	return _contactModifyCallback;
}
void Scene::ContactModifyCallback::set(PhysX::ContactModifyCallback^ value)
{
	_contactModifyCallback = value;

	_scene->setContactModifyCallback(value == nullptr ? NULL : value->UnmanagedPointer);
}

PxScene* Scene::UnmanagedPointer::get()
{
	return _scene;
}