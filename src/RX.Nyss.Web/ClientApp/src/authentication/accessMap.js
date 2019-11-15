import { Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer } from "./roles";

export const accessMap = {
  nationalSocieties: {
    list: [Administrator, GlobalCoordinator, TechnicalAdvisor, DataConsumer],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    delete: [Administrator, GlobalCoordinator],
    get: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer],
  },
  smsGateways: {
    list: [Administrator, Manager, TechnicalAdvisor],
    add: [Administrator, Manager, TechnicalAdvisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
    delete: [Administrator, Manager, TechnicalAdvisor]
  },
  nationalSocietyUsers: {
    list: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    add: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    edit: [Administrator, GlobalCoordinator,  Manager, TechnicalAdvisor],
    delete: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor]
  },
  globalCoordinators: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator],
    edit: [Administrator],
    delete: [Administrator]
  },
  healthRisks: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator],
    delete: [Administrator, GlobalCoordinator]
  },
  dataCollectors: {
    list: [Administrator, Manager, TechnicalAdvisor],
    add: [Administrator, Manager, TechnicalAdvisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
    delete: [Administrator, Manager, TechnicalAdvisor]
  },
  projects: {
    list: [Administrator, Manager, TechnicalAdvisor],
    add: [Administrator, Manager, TechnicalAdvisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
    delete: [Administrator, Manager, TechnicalAdvisor]
  },
};
