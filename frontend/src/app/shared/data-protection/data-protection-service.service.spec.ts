import { TestBed } from '@angular/core/testing';

import { DataProtectionHelperService } from './data-protection-service.service';

describe('DataProtectionService', () => {
  let service: DataProtectionHelperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DataProtectionHelperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
