import { TestBed } from '@angular/core/testing';

import { OsmService } from './osm.service';
import { AddressDto } from '../api-generated/api-generated';
import { testOSM } from './osm-testdata';
import { of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

describe('OsmService', () => {
  let service: OsmService;
  let httpServiceMock;

  beforeEach(() => {
    httpServiceMock = jasmine.createSpyObj('HttpClient', ['get']);
    httpServiceMock.get.and.returnValue(of(testOSM));

    TestBed.configureTestingModule({
      providers: [{
        provide: HttpClient, useValue: httpServiceMock
      }]
    });
    service = TestBed.inject(OsmService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should parse address json', () => {
    const address: AddressDto = OsmService.parseAddress(testOSM);

    const expected: AddressDto = {
      country: 'Deutschland',
      city: 'Berlin',
      zipcode: '10715',
      street: 'Berliner Straße',
      streetNumber: undefined
    };

    expect(address).toEqual(expected);
  });

  it('should get address from mock', () => {
    const result = service.getResults('test');

    return result.subscribe(res => {
      expect(httpServiceMock.get).toHaveBeenCalled();

      expect(httpServiceMock.get).toHaveBeenCalledWith('http://photon.komoot.de/api?q=test&lang=de');
      expect(res).toEqual(testOSM);
    });
  });
});
