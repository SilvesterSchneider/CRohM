export interface Contact {
    id: number;
    nachname: string;
    vorname: string;
    adresse: {
        strasse: string;
        plz: string;
        ort: string;
    };
    firma?: string;
}
