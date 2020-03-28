export interface Contact {
    id: number;
    nachname: string;
    vorname: string;
    adresse: {
        land: string;
        strasse: string;
        plz: string;
        ort: string;
    };
    mail: string;
    phone: string;
    firma?: string;
}



export interface Country {
    value: string;
    viewValue: string;
  }

