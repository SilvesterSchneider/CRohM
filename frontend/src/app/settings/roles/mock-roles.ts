
export interface IRoleTemp {
    id: number;
    name: string;
    permissions: string[];
  }

export interface IPermissionTemp {
    name: string;
  }

export const PERMISSIONS: IPermissionTemp[] = [
    {
        name: 'Löschen von Kontakte'
    },
    {
        name: 'Hinzufügen neuer Kontakte'
    },
    {
        name: 'Lesen von Kontakt'
    }
];

export const ROLES: IRoleTemp[] = [
    {
        id: 1,
        name: 'Administrator',
        permissions: []
    },
    {
        id: 2,
        name: 'Datenschutzbeauftragter',
        permissions: ['Löschen von Kontakte']
    },
    {
        id: 3,
        name: 'Rolle X',
        permissions: ['Hinzufügen neuer Kontakte', 'Lesen von Kontakt']
    },
    {
        id: 4,
        name: 'Rolle Y',
        permissions: ['Hinzufügen neuer Kontakte', 'Lesen von Kontakt']
    },
    {
        id: 5,
        name: 'Rolle Z',
        permissions: ['Hinzufügen neuer Kontakte']
    }
];
