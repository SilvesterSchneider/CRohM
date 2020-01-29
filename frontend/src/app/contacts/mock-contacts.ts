import { Contact } from './contacts.model';

export const CONTACTS: Contact[] = [
    {
        id: 0,
        vorname: 'Max',
        nachname: 'Mustermann',
        adresse: {
            strasse: 'Teststrasse 1',
            ort: 'Nürnberg',
            plz: '12345'
        }
    },
    {
        id: 1,
        vorname: 'Judith',
        nachname: 'Androthe',
        adresse: {
            strasse: 'Teststrasse 2',
            ort: 'Nürnberg',
            plz: '12345'
        }
    },
    {
        id: 2,
        vorname: 'Julia',
        nachname: 'Stoh',
        adresse: {
            strasse: 'Teststrasse 3',
            ort: 'Nürnberg',
            plz: '12345'
        }
    },
    {
        id: 3,
        vorname: 'Andreas',
        nachname: 'Meier',
        adresse: {
            strasse: 'Teststrasse 15',
            ort: 'Nürnberg',
            plz: '12345'
        }
    }
];
