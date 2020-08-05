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

  public getDiffOfObjects(newObj, oldObj, filter: string[]): any {

    if (this.isFunction(newObj) || this.isFunction(oldObj)) {
      throw new Error('Invalid argument. Function given, object expected.');
    }
    if (this.isValue(newObj) || this.isValue(oldObj)) {

      const retVal = {
        type: this.compareValues(newObj, oldObj),
        data: newObj === undefined ? oldObj : newObj
      };

      return retVal;
    }

    const diff = {};

    for (const key in newObj) {
      if (this.isFunction(newObj[key])) {
        continue;
      }

      let value2;
      if (oldObj[key] !== undefined) {
        value2 = oldObj[key];
      }

      const obj = this.getDiffOfObjects(newObj[key], value2, filter);

      if (filter.includes(obj.type)) {
        delete oldObj[key];
        continue;

      }
      diff[key] = obj;



    }
    for (const key in oldObj) {
      if (this.isFunction(oldObj[key]) || diff[key] !== undefined) {
        continue;
      }

      const obj = this.getDiffOfObjects(undefined, oldObj[key], filter);
      if (obj != null) {
        diff[key] = obj;

      }
    }

    for (const key in diff) {
      if (Object.keys(diff[key]).length === 0) {
        delete diff[key];
      }
    }
    return diff;
  }

  private compareValues(newValue, oldValue): string {
    if (newValue === oldValue) {
      return this.VALUE_UNCHANGED;
    }
    if (this.isDate(newValue) && this.isDate(oldValue) && newValue.getTime() === oldValue.getTime()) {
      return this.VALUE_UNCHANGED;
    }
    if (newValue === undefined) {
      return this.VALUE_DELETED;
    }
    if (oldValue === undefined) {
      return this.VALUE_CREATED;
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
