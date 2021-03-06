import * as actions from "./reportsConstants";
import * as projectsActions from "../../projects/logic/projectsConstants";
import { initialState } from "../../../initialState";
import { LOCATION_CHANGE } from "connected-react-router";

export function reportsReducer(state = initialState.reports, action) {
  switch (action.type) {
    case projectsActions.CLOSE_PROJECT.SUCCESS:
      return { ...state, listStale: true, listProjectId: null };

    case LOCATION_CHANGE: // cleanup
      return { ...state, formData: null }

    case actions.OPEN_REPORTS_LIST.INVOKE:
      return { ...state, listStale: state.listStale || action.projectId !== state.listProjectId };

    case actions.OPEN_REPORTS_LIST.SUCCESS:
      return { ...state, listProjectId: action.projectId, filtersData: action.filtersData };

    case actions.GET_REPORTS.REQUEST:
      return { ...state, paginatedListData: state.listStale ? null : state.paginatedListData, listFetching: true };

    case actions.GET_REPORTS.SUCCESS:
      return { ...state, filters: action.filters, sorting: action.sorting, listFetching: false, listStale: false, paginatedListData: { data: action.data, page: action.page, rowsPerPage: action.rowsPerPage, totalRows: action.totalRows } };

    case actions.GET_REPORTS.FAILURE:
      return { ...state, listFetching: false, paginatedListData: null };

    case actions.OPEN_REPORT_EDITION.INVOKE:
      return { ...state, formFetching: true, formData: null };

    case actions.OPEN_REPORT_EDITION.REQUEST:
      return { ...state, formFetching: true, formData: null };

    case actions.OPEN_REPORT_EDITION.SUCCESS:
      return { ...state, formFetching: false, formData: action.data, formHealthRisks: action.healthRisks };

    case actions.OPEN_REPORT_EDITION.FAILURE:
      return { ...state, formFetching: false };

    case actions.EDIT_REPORT.REQUEST:
      return { ...state, formSaving: true };

    case actions.EDIT_REPORT.SUCCESS:
      return { ...state, formSaving: false, listStale: true };

    case actions.EDIT_REPORT.FAILURE:
      return { ...state, formSaving: false };

    case actions.MARK_AS_ERROR.REQUEST:
      return { ...state, markingAsError: true };

    case actions.MARK_AS_ERROR.SUCCESS:
      return { ...state, markingAsError: false };

    case actions.MARK_AS_ERROR.FAILURE:
      return { ...state, markingAsError: false, message: action.message };

    default:
      return state;
  }
};
