import styles from '../common/table/Table.module.scss';
import React from 'react';
import PropTypes from "prop-types";
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import { Loading } from '../common/loading/Loading';
import { strings, stringKeys } from '../../strings';
import TablePagination from '@material-ui/core/TablePagination';
import  TableFooter  from '@material-ui/core/TableFooter';
import dayjs from "dayjs";

export const ReportsTable = ({ isListFetching, list, projectId, getList, page, rowsPerPage, totalRows }) => {

  const onChangePage = (event, page) => {
    getList(projectId, page + 1);
  };

  const dashIfEmpty = (text) => {
    return text || "-";
  };

  const getSuccessText = (isValid) => {
    return isValid
    ? strings(stringKeys.reports.list.success)
    : strings(stringKeys.reports.list.error);
  };

  if (isListFetching) {
    return <Loading />;
  }

  return (
    <Table>
      <TableHead>
        <TableRow>
          <TableCell style={{ width: "6%", "minWidth": "100px" }}>{strings(stringKeys.reports.list.date)}</TableCell>
          <TableCell style={{ width: "5%" }}>{strings(stringKeys.reports.list.time)}</TableCell>
          <TableCell style={{ width: "10%" }}>{strings(stringKeys.reports.list.healthRisk)}</TableCell>
          <TableCell style={{ width: "6%" }}>{strings(stringKeys.reports.list.status)}</TableCell>
          <TableCell style={{ width: "18%", "minWidth": "250px" }}>{strings(stringKeys.reports.list.location)}</TableCell>
          <TableCell style={{ width: "14%", "minWidth": "200px" }}>{strings(stringKeys.reports.list.dataCollectorDisplayName)}</TableCell>
          <TableCell style={{ width: "11%" }}>{strings(stringKeys.reports.list.dataCollectorPhoneNumber)}</TableCell>
          <TableCell style={{ width: "5%", "minWidth": "50px" }}>{strings(stringKeys.reports.list.malesBelowFive)}</TableCell>
          <TableCell style={{ width: "5%", "minWidth": "50px" }}>{strings(stringKeys.reports.list.malesAtLeastFive)}</TableCell>
          <TableCell style={{ width: "5%", "minWidth": "50px" }}>{strings(stringKeys.reports.list.femalesBelowFive)}</TableCell>
          <TableCell style={{ width: "5%", "minWidth": "50px" }}>{strings(stringKeys.reports.list.femalesAtLeastFive)}</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {list.map(row => (
          <TableRow key={row.id} hover className={styles.clickableRow}>
            <TableCell>{dayjs(row.dateTime).format('YYYY-MM-DD')}</TableCell>
            <TableCell>{dayjs(row.dateTime).format('hh:mm')}</TableCell>
            <TableCell>{dashIfEmpty(row.healthRiskName)}</TableCell>
            <TableCell>{getSuccessText(row.isValid)}</TableCell>          
            <TableCell>{row.region}, {row.district}, {row.village}{row.zone ? ',' : null} {row.zone}</TableCell>
            <TableCell>{row.dataCollectorDisplayName}</TableCell>
            <TableCell>{row.dataCollectorPhoneNumber}</TableCell>
            <TableCell>{dashIfEmpty(row.countMalesBelowFive)}</TableCell>
            <TableCell>{dashIfEmpty(row.countMalesAtLeastFive)}</TableCell>
            <TableCell>{dashIfEmpty(row.countFemalesBelowFive)}</TableCell>
            <TableCell>{dashIfEmpty(row.countFemalesAtLeastFive)}</TableCell>
          </TableRow>
        ))}
      </TableBody>

      <TableFooter>
        <TableRow>
          <TablePagination count={totalRows} rowsPerPage={rowsPerPage} page={page - 1} onChangePage={onChangePage} />
        </TableRow>
      </TableFooter>
    </Table>
  );
}

ReportsTable.propTypes = {
  isFetching: PropTypes.bool,
  list: PropTypes.array
};

export default ReportsTable;