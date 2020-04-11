import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AddressDto } from '../api-generated/api-generated';


export const API_URL = 'http://photon.komoot.de/api';

@Injectable({
  providedIn: 'root'
})
export class OsmService {

  public static parseAddress(placeResult: any): AddressDto {
    const address: AddressDto = {
      country: placeResult.properties.country,
      city: placeResult.properties.city,
      zipcode: placeResult.properties.zipcode,
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
