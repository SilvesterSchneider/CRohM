import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class RolesTranslationService {

    public static PERMISSIONS = [
        {
            value: 'Löschen eines Kontakts',
            label: 'settings.permission.deleteContact'
        },
        {
            value: 'Anlegen eines Benutzers',
            label: 'settings.permission.createUser'
        },
        {
            value: 'Anlegen eines Kontakts',
            label: 'settings.permission.createContact'
        },
        {
            value: 'Zuweisung einer neuen Rolle zu einem Benutzer',
            label: 'settings.permission.mapRoleUser'
        },
        {
            value: 'Rücksetzen eines Passworts eines Benutzers',
            label: 'settings.permission.resetPassword'
        },
        {
            value: 'Löschen / Deaktivieren eines Benutzers',
            label: 'settings.permission.deleteUser'
        },
        {
            value: 'Einsehen und Überarbeiten des Rollenkonzepts',
            label: 'settings.permission.changeRoleConcept'
        },
        {
            value: 'Einsehen und Bearbeiten aller Kontakte',
            label: 'settings.permission.editAllContacts'
        },
        {
            value: 'Auskunft gegenüber eines Kontakts zu dessen Daten',
            label: 'settings.permission.disclosure'
        },
        {
            value: 'Mitteilung an einen Kontakt nach Löschung oder Änderung',
            label: 'settings.permission.messageContact'
        },
        {
            value: 'Anlegen einer Organisation',
            label: 'settings.permission.createOrganization'

        },
        {
            value: 'Einsehen und Bearbeiten aller Organisationen',
            label: 'settings.permission.editAllOrganization'
        },
        {
            value: 'Zuordnen eines Kontakts zu einer Organisation',
            label: 'settings.permission.mapContactOrganization'
        },
        {
            value: 'Löschen einer Organisation',
            label: 'settings.permission.deleteOrganization'
        },
        {
            value: 'Hinzufügen eines Historieneintrags bei Kontakt oder Organisation',
            label: 'settings.permission.createHistory'
        },
        {
            value: 'Anlegen einer Veranstaltung',
            label: 'settings.permission.createEvent'
        },
        {
            value: 'Einsehen und Bearbeiten einer Veranstaltung',
            label: 'settings.permission.editEvent'
        },
        {
            value: 'Löschen einer Veranstaltung',
            label: 'settings.permission.deleteEvent'
        }
    ];

    public static ROLES = [{
        value: 'Admin',
        label: 'settings.role.admin'
    }, {
        value: 'Datenschutzbeauftragter',
        label: 'settings.role.datasecurity'
    }];

    public static mapRole(role: string) {
        return RolesTranslationService.ROLES.find(r => r.value === role) ?? { value: role, label: role };
    }

    public static mapPermission(permission: string) {
        return RolesTranslationService.PERMISSIONS.find(r => r.value === permission) ?? { value: permission, label: permission };
    }
}
