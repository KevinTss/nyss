import { placeholders } from "../../../siteMapPlaceholders";
import { accessMap } from "../../../authentication/accessMap";
import { stringKeys, strings } from "../../../strings";

export const globalCoordinatorsSiteMap = [
  {
    path: "/globalcoordinators",
    title: () => strings(stringKeys.globalCoordinator.title),
    placeholder: placeholders.topMenu,
    access: accessMap.globalCoordinators.list
  },
  {
    parentPath: "/globalcoordinators",
    path: "/globalcoordinators/add",
    title: () => strings(stringKeys.globalCoordinator.form.creationTitle),
    access: accessMap.globalCoordinators.add
  },
  {
    parentPath: "/globalcoordinators",
    path: "/globalcoordinators/:globalCoordinatorId/edit",
    title: () => strings(stringKeys.globalCoordinator.form.editionTitle),
    access: accessMap.globalCoordinators.edit
  }
];
