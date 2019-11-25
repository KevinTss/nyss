import { call, put, takeEvery } from "redux-saga/effects";
import * as consts from "./nationalSocietyStructureConstants";
import * as actions from "./nationalSocietyStructureActions";
import * as appActions from "../../app/logic/appActions";
import * as http from "../../../utils/http";
import { entityTypes } from "../../nationalSocieties/logic/nationalSocietiesConstants";

export const nationalSocietyStructureSagas = () => [
  takeEvery(consts.OPEN_NATIONAL_SOCIETY_STRUCTURE.INVOKE, openNationalSocietyStructure),

  takeEvery(consts.CREATE_REGION.INVOKE, createRegion),
  takeEvery(consts.EDIT_REGION.INVOKE, editRegion),
  takeEvery(consts.REMOVE_REGION.INVOKE, removeRegion),

  takeEvery(consts.CREATE_DISTRICT.INVOKE, createDistrict),
  takeEvery(consts.EDIT_DISTRICT.INVOKE, editDistrict),
  takeEvery(consts.REMOVE_DISTRICT.INVOKE, removeDistrict),

  takeEvery(consts.CREATE_VILLAGE.INVOKE, createVillage),
  takeEvery(consts.EDIT_VILLAGE.INVOKE, editVillage),
  takeEvery(consts.REMOVE_VILLAGE.INVOKE, removeVillage),

  takeEvery(consts.CREATE_ZONE.INVOKE, createZone),
  takeEvery(consts.EDIT_ZONE.INVOKE, editZone),
  takeEvery(consts.REMOVE_ZONE.INVOKE, removeZone)
];

function* openNationalSocietyStructure({ nationalSocietyId }) {
  yield put(actions.openStructure.request());
  try {
    yield call(openNationalSocietyModule, nationalSocietyId);
    const response = yield call(http.get, `/api/nationalSociety/${nationalSocietyId}/structure/get`);

    let regions = [];
    let districts = [];
    let villages = [];
    let zones = [];

    for (let region of response.value.regions) {
      regions.push({ id: region.id, name: region.name })
      for (let district of region.districts) {
        districts.push({ id: district.id, name: district.name, regionId: region.id })
        for (let village of district.villages) {
          villages.push({ id: village.id, name: village.name, districtId: district.id })
          zones = village.zones.map(zone => ({ id: zone.id, name: zone.name, villageId: village.id }))
        }
      }
    }

    yield put(actions.openStructure.success(regions, districts, villages, zones));
  } catch (error) {
    yield put(actions.openStructure.failure(error.message));
  }
};

function* createRegion({ nationalSocietyId, name }) {
  yield put(actions.createRegion.request());
  try {
    const response = yield call(http.post, `/api/nationalSociety/${nationalSocietyId}/region/create`, { name });
    yield put(actions.createRegion.success(response.value));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.createRegion.failure(error.message));
  }
};

function* editRegion({ id, name }) {
  yield put(actions.editRegion.request());
  try {
    yield call(http.post, `/api/region/${id}/edit`, { name });
    yield put(actions.editRegion.success(id, name));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.editRegion.failure(error.message));
  }
};

function* removeRegion({ id }) {
  yield put(actions.removeRegion.request());
  try {
    yield call(http.post, `/api/region/${id}/remove`);
    yield put(actions.removeRegion.success(id));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.removeRegion.failure(error.message));
  }
};

function* createDistrict({ regionId, name }) {
  yield put(actions.createDistrict.request());
  try {
    const response = yield call(http.post, `/api/region/${regionId}/district/create`, { name });
    yield put(actions.createDistrict.success(regionId, response.value));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.createDistrict.failure(error.message));
  }
};

function* editDistrict({ id, name }) {
  yield put(actions.editDistrict.request());
  try {
    yield call(http.post, `/api/district/${id}/edit`, { name });
    yield put(actions.editDistrict.success(id, name));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.editDistrict.failure(error.message));
  }
};

function* removeDistrict({ id }) {
  yield put(actions.removeDistrict.request());
  try {
    yield call(http.post, `/api/district/${id}/remove`);
    yield put(actions.removeDistrict.success(id));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.removeDistrict.failure(error.message));
  }
};

function* createVillage({ districtId, name }) {
  yield put(actions.createVillage.request());
  try {
    const response = yield call(http.post, `/api/district/${districtId}/village/create`, { name });
    yield put(actions.createVillage.success(response.value));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.createVillage.failure(error.message));
  }
};

function* editVillage({ id, name }) {
  yield put(actions.editVillage.request());
  try {
    yield call(http.post, `/api/village/${id}/edit`, { name });
    yield put(actions.editVillage.success(id, name));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.editVillage.failure(error.message));
  }
};

function* removeVillage({ id }) {
  yield put(actions.removeVillage.request());
  try {
    yield call(http.post, `/api/village/${id}/remove`);
    yield put(actions.removeVillage.success(id));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.removeVillage.failure(error.message));
  }
};

function* createZone({ villageId, name }) {
  yield put(actions.createZone.request());
  try {
    const response = yield call(http.post, `/api/village/${villageId}/zone/create`, { name });
    yield put(actions.createZone.success(response.value));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.createZone.failure(error.message));
  }
};

function* editZone({ id, name }) {
  yield put(actions.editZone.request());
  try {
    yield call(http.post, `/api/zone/${id}/edit`, { name });
    yield put(actions.editZone.success(id, name));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.editZone.failure(error.message));
  }
};

function* removeZone({ id }) {
  yield put(actions.removeZone.request());
  try {
    yield call(http.post, `/api/zone/${id}/remove`);
    yield put(actions.removeZone.success(id));
  } catch (error) {
    yield put(appActions.showMessage(error.message));
    yield put(actions.removeZone.failure(error.message));
  }
};

function* openNationalSocietyModule(nationalSocietyId) {
  const nationalSociety = yield call(http.getCached, {
    path: `/api/nationalSociety/${nationalSocietyId}/get`,
    dependencies: [entityTypes.nationalSociety(nationalSocietyId)]
  });

  yield put(appActions.openModule.invoke(null, {
    nationalSocietyId: nationalSociety.value.id,
    nationalSocietyName: nationalSociety.value.name,
    nationalSocietyCountry: nationalSociety.value.countryName
  }));
}