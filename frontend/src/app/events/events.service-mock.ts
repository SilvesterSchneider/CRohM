import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { EventDto, ContactDto } from '../shared/api-generated/api-generated';
import { Injectable } from '@angular/core';
import { EVENTS } from './mock-events';
import { EventCreateDto } from '../shared/api-generated/api-generated';

@Injectable({
    providedIn: 'root',
})
export class EventsServiceMock {
    static nextId = 4;
    private events$: BehaviorSubject<EventDto[]> = new BehaviorSubject<EventDto[]>(EVENTS);
    private contactInner: EventDto;
    constructor() { }

    getEvents() {
        return this.events$.asObservable();
    }

    getById(id: number | string) {
        return this.getEvents().pipe(
            map(contacts => contacts.find(contact => contact.id === +id))
        );
    }

    addEvent(eventInner: EventCreateDto) {
        const contactsToSet: ContactDto[] = new Array();
        eventInner.contacts.forEach(x => contactsToSet.push(
            {
                id: x
            }
        ));
        this.contactInner = {
            id: EventsServiceMock.nextId++,
            name: eventInner.name,
            date: eventInner.date,
            time: eventInner.time,
            duration: eventInner.duration,
            contacts: contactsToSet
        };
        EVENTS.push(this.contactInner);
        this.events$.next(EVENTS);
    }

    delete(id: number) {
        const index: number = EVENTS.findIndex(x => x.id === id);
        if (index > -1) {
            EVENTS.splice(index, 1);
        }
    }

    update(eventInner: EventDto, id: number) {
        this.delete(id);
        EVENTS.push(eventInner);
        this.events$.next(EVENTS);
    }
}
