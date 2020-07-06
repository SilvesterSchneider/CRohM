import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DataProtectionHelperService {
  private readonly VALUE_CREATED: string = 'created';
  private readonly VALUE_UPDATED: string = 'updated';
  private readonly VALUE_DELETED: string = 'deleted';
  private readonly VALUE_UNCHANGED: string = 'unchanged';

  constructor() { }

  public getDiffOfObjects(obj1, obj2, filter: string[]): any {

    if (this.isFunction(obj1) || this.isFunction(obj2)) {
          throw new Error('Invalid argument. Function given, object expected.');
        }
    if (this.isValue(obj1) || this.isValue(obj2)) {

          const retVal = {
          type: this.compareValues(obj1, obj2),
          data: obj1 === undefined ? obj2 : obj1
          };

          return retVal;
        }

    const diff = {};
    for (const key in obj1) {
          if (this.isFunction(obj1[key])) {
          continue;
          }

          let value2;
          if (obj2[key] !== undefined) {
          value2 = obj2[key];
          }

          const obj = this.getDiffOfObjects(obj1[key], value2, filter);

          if (filter.includes(obj.type)) {
            delete  obj2[key];
            continue;

          }
          diff[key] = obj;


        }
    for (const key in obj2) {
          if (this.isFunction(obj2[key]) || diff[key] !== undefined) {
          continue;
          }

          const obj = this.getDiffOfObjects(undefined, obj2[key], filter);
          if (obj != null) {
            diff[key] = obj;
          }
        }

    for (const key in diff) {
      if (Object.keys(diff[key]).length === 0 ) {
        delete diff[key];
      }
    }
    return diff;
  }

  private compareValues(value1, value2): string {
    if (value1 === value2) {
      return this.VALUE_UNCHANGED;
    }
    if (this.isDate(value1) && this.isDate(value2) && value1.getTime() === value2.getTime()) {
      return this.VALUE_UNCHANGED;
    }
    if (value1 === undefined) {
      return this.VALUE_CREATED;
    }
    if (value2 === undefined) {
      return this.VALUE_DELETED;
    }
    return this.VALUE_UPDATED;
    }

  private isFunction(x): boolean {
    return Object.prototype.toString.call(x) === '[object Function]';
  }

  private isArray(x): boolean {
    return Object.prototype.toString.call(x) === '[object Array]';
  }

  private isDate(x): boolean {
    return Object.prototype.toString.call(x) === '[object Date]';
  }

  private isObject(x): boolean {
    return Object.prototype.toString.call(x) === '[object Object]';
  }

  private isValue(x): boolean {
    return !this.isObject(x) && !this.isArray(x);
  }
}
