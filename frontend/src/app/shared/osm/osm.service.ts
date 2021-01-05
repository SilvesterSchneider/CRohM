import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AddressDto } from '../api-generated/api-generated';


export const API_URL = 'https://photon.komoot.io/api';

@Injectable({
  providedIn: 'root'
})
export class OsmService {

  public static parseAddress(placeResult: any): AddressDto {
    const address: AddressDto = {
      id: placeResult.properties.id,
      country: placeResult.properties.country,
      city: placeResult.properties.city,
      zipcode: placeResult.properties.postcode,
      street: placeResult.properties.name,
      streetNumber: placeResult.properties.number,
    };
    return address;
  }

  constructor(private http: HttpClient) { }

  public getResults(seachTerm: string) {
    return this.http.get(`${API_URL}?q=${seachTerm}&lang=de`);
  }


}
