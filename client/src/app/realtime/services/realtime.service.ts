import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Observable, Subject } from 'rxjs';
import { TokenStore } from '../../login/models/token-store';
import { ItemAdded, ItemDeleted, ItemDoneChanged, ItemNameChanged, ListAdded, ListDeleted, ListRenamed } from '../models/realtime-models';
import { RealtimeModuleConfiguration } from '../models/realtime-module-configuration';

@Injectable()
export class RealtimeService {
  listAdded$ = new Observable<ListAdded>();
  listDeleted$ = new Observable<ListDeleted>();
  listRenamed$ = new Observable<ListRenamed>();
  itemAdded$ = new Observable<ItemAdded>();
  itemNameChanged$ = new Observable<ItemNameChanged>();
  itemDoneChanged$ = new Observable<ItemDoneChanged>();
  itemDeleted$ = new Observable<ItemDeleted>();

  private connection: signalR.HubConnection;
  private listAddedSubject = new Subject<ListAdded>();
  private listDeletedSubject = new Subject<ListDeleted>();
  private listRenamedSubject = new Subject<ListRenamed>();
  private itemAddedSubject = new Subject<ItemAdded>();
  private itemNameChangedSubject = new Subject<ItemNameChanged>();
  private itemDoneChangedSubject = new Subject<ItemDoneChanged>();
  private itemDeletedSubject = new Subject<ItemDeleted>();

  constructor(private readonly moduleConfiguration: RealtimeModuleConfiguration, private readonly tokenStore: TokenStore) {
    this.listAdded$ = this.listAddedSubject.asObservable();
    this.listDeleted$ = this.listDeletedSubject.asObservable();
    this.listRenamed$ = this.listRenamedSubject.asObservable();
    this.itemAdded$ = this.itemAddedSubject.asObservable();
    this.itemNameChanged$ = this.itemNameChangedSubject.asObservable();
    this.itemDoneChanged$ = this.itemDoneChangedSubject.asObservable();
    this.itemDeleted$ = this.itemDeletedSubject.asObservable();
  }

  async connect(): Promise<void> {
    if (this.connection) {
      await this.disconnect();
    }

    this.connection = new signalR.HubConnection(`${this.moduleConfiguration.hubUrl}/hubs/list?token=${this.tokenStore.accessToken}`);
    this.assignEvents();

    return this.connection.start();
  }

  disconnect(): Promise<void> {
    this.removeEvents();

    return this.connection.stop()
      .then(() => this.connection = void 0);
  }

  private assignEvents(): void {
    this.connection.on('listAdded', this.listAdded.bind(this));
    this.connection.on('listDeleted', this.listDeleted.bind(this));
    this.connection.on('listRenamed', this.listRenamed.bind(this));
    this.connection.on('itemAdded', this.itemAdded.bind(this));
    this.connection.on('itemNameChanged', this.itemNameChanged.bind(this));
    this.connection.on('itemDoneChanged', this.itemDoneChanged.bind(this));
    this.connection.on('itemDeleted', this.itemDeleted.bind(this));
  }

  private removeEvents(): void {
    this.connection.off('listAdded', this.listAdded.bind(this));
    this.connection.off('listDeleted', this.listDeleted.bind(this));
    this.connection.off('listRenamed', this.listRenamed.bind(this));
    this.connection.off('itemAdded', this.itemAdded.bind(this));
    this.connection.off('itemNameChanged', this.itemNameChanged.bind(this));
    this.connection.off('itemDoneChanged', this.itemDoneChanged.bind(this));
    this.connection.off('itemDeleted', this.itemDeleted.bind(this));
  }

  private listAdded(listId: number, listName: string): void {
    this.listAddedSubject.next({ listId, listName });
  }

  private listDeleted(listId: number): void {
    this.listDeletedSubject.next({ listId });
  }

  private listRenamed(listId: number, newName: string): void {
    this.listRenamedSubject.next({ listId, newName });
  }

  private itemAdded(listId: number, itemId: number, itemName: string): void {
    this.itemAddedSubject.next({ listId, itemId, itemName });
  }

  private itemNameChanged(listId: number, itemId: number, newName: string): void {
    this.itemNameChangedSubject.next({ listId, itemId, newName });
  }

  private itemDoneChanged(listId: number, itemId: number, done: boolean): void {
    this.itemDoneChangedSubject.next({ listId, itemId, done });
  }

  private itemDeleted(listId: number, itemId: number): void {
    this.itemDeletedSubject.next({ listId, itemId });
  }
}
