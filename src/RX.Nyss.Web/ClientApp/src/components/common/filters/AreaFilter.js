import styles from "./AreaFilter.module.scss";

import React, { Fragment, useRef, useState } from 'react';
import Menu from "@material-ui/core/Menu";
import { get } from '../../../utils/http';
import TreeView from '@material-ui/lab/TreeView';
import TreeItem from '@material-ui/lab/TreeItem';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import InputAdornment from '@material-ui/core/InputAdornment';
import ArrowDropDown from '@material-ui/icons/ArrowDropDown';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import TextField from "@material-ui/core/TextField";
import { strings, stringKeys } from "../../../strings";

const StructureTreeItem = ({ nodeId, data, label, onSelect, isSelected, children }) => {
  const handleChange = (e) => {
    e.stopPropagation();
    onSelect(data);
  }

  const treeItemContent = () => (
    <div onClick={handleChange}>
      {data.name}
    </div>
  );

  return (
    <TreeItem
      nodeId={`${data.type}_${data.id}`}
      className={isSelected ? styles.selected : null}
      label={treeItemContent()}>
      {children}
    </TreeItem>
  );
}

export const AreaFilter = ({ nationalSocietyId, selectedItem, onChange }) => {
  const [dropdownVisible, setDropdownVisible] = useState(false);
  const [, setIsFetching] = useState(false);
  const [structureLoaded, setStructureLoaded] = useState(false);
  const [regions, setRegions] = useState([]);
  const triggerRef = useRef(null);

  const handleDropdownClick = (e) => {
    setDropdownVisible(true);

    if (!structureLoaded) {
      setIsFetching(true);
      get(`/api/nationalSocietyStructure/get?nationalSocietyId=${nationalSocietyId}`)
        .then(response => {
          setIsFetching(false);
          setStructureLoaded(true);
          setRegions(response.value.regions)
        })
        .catch(() => {
          setIsFetching(false);
        })
    }
  }

  const handleDropdownClose = (e) => {
    setDropdownVisible(false);
  };

  const handleChange = (item) => {
    onChange(item);
    setDropdownVisible(false);
  }

  const selectedItemKey = selectedItem ? `${selectedItem.type}_${selectedItem.id}` : null;
  const selectedItemParents = selectedItem ? selectedItem.parents : [];

  if (!regions) {
    return null;
  }

  const handleChangeToEmpty = (e) => {
    e.stopPropagation();
    handleChange(null);
  }

  return (
    <Fragment>
      <TextField
        onClick={handleDropdownClick}
        ref={triggerRef}
        className={styles.field}
        label={strings(stringKeys.filters.area.title)}
        InputProps={{
          readOnly: true,
          endAdornment: (
            <InputAdornment >
              <ArrowDropDown className={styles.arrow} />
            </InputAdornment>
          )
        }}
        inputProps={{
          className: styles.areaSelect
        }}
        value={selectedItem ? selectedItem.name : strings(stringKeys.filters.area.all)}
      />
      <Menu
        anchorEl={triggerRef.current}
        onClose={handleDropdownClose}
        open={dropdownVisible}
        className={styles.dropDown}
        PaperProps={{ className: styles.dropDown }}
      >
        <TreeView
          className={styles.tree}
          defaultCollapseIcon={<ExpandMoreIcon />}
          defaultExpandIcon={<ChevronRightIcon />}
          defaultExpanded={selectedItemParents}
        >
          <Fragment>
            <TreeItem
              className={!selectedItemKey ? styles.selected : null}
              nodeId="none"
              onClick={handleChangeToEmpty}
              label={strings(stringKeys.filters.area.all)}>
            </TreeItem>

            {regions.map(region => ({ key: `region_${region.id}`, data: region })).map(region => (
              <StructureTreeItem
                key={region.key}
                data={{ type: "region", id: region.data.id, name: region.data.name, parents: [] }}
                isSelected={selectedItemKey === region.key}
                onSelect={handleChange}
                label={region.name}>

                {region.data.districts.map(district => ({ key: `district_${district.id}`, data: district })).map(district => (
                  <StructureTreeItem
                    key={district.key}
                    data={{ type: "district", id: district.data.id, name: district.data.name, parents: [region.key] }}
                    isSelected={selectedItemKey === district.key}
                    onSelect={handleChange}
                    label={district.name}>
                    {district.data.villages.map(village => ({ key: `village_${village.id}`, data: village })).map(village => (
                      <StructureTreeItem
                        key={village.key}
                        data={{ type: "village", id: village.data.id, name: village.data.name, parents: [region.key, district.key] }}
                        isSelected={selectedItemKey === village.key}
                        onSelect={handleChange}
                        label={village.name}>
                        {village.data.zones.map(zone => ({ key: `zone_${zone.id}`, data: zone })).map(zone => (
                          <StructureTreeItem
                            key={zone.key}
                            data={{ type: "zone", id: zone.data.id, name: zone.data.name, parents: [region.key, district.key, village.key] }}
                            isSelected={selectedItemKey === zone.key}
                            onSelect={handleChange}
                            label={zone.name} />
                        ))}
                      </StructureTreeItem>
                    ))}
                  </StructureTreeItem>
                ))}
              </StructureTreeItem>
            ))}
          </Fragment>
        </TreeView>
      </Menu>
    </Fragment>
  );
}
