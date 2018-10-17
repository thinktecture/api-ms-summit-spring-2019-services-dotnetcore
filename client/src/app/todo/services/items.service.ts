import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { forkJoin } from 'rxjs/observable/forkJoin';
import { map } from 'rxjs/operators';
import { TodoItem } from '../models/todo-item';
import { TodoList } from '../models/todo-list';
import { TodoModuleConfiguration } from '../models/todo-module-configuration';

@Injectable()
export class ItemsService {
  constructor(private readonly http: HttpClient, private readonly configuration: TodoModuleConfiguration) {
  }

  list(listId: number): Observable<TodoList> {
    return forkJoin(
      this.http.get<{value: string}>(`${this.configuration.apiUrl}/list/${listId}`),
      this.http.get<TodoItem[]>(`${this.configuration.apiUrl}/list/${listId}/item`),
    ).pipe(
      map(([todoListName, todoItems]) => ({ id: listId, name: todoListName.value, items: todoItems } as TodoList)),
    );
  }

  createItem(listId: number, name: string): Observable<number> {
    return this.http.post<{id: number}>(`${this.configuration.apiUrl}/list/${listId}/item`, { value: name }).pipe(
      map(result => result.id),
    );
  }

  updateItem(listId: number, item: TodoItem): Observable<void> {
    return this.http.put(`${this.configuration.apiUrl}/list/${listId}/item/${item.id}`, { value: item.text })
      .pipe(map(() => void 0));
  }

  removeItem(listId: number, id: number): Observable<void> {
    return this.http.delete(`${this.configuration.apiUrl}/list/${listId}/item/${id}`)
      .pipe(map(() => void 0));
  }

  toggleItem(listId: number, id: number): Observable<void> {
    return this.http.post(`${this.configuration.apiUrl}/list/${listId}/item/${id}/toggle`, void 0)
      .pipe(map(() => void 0));
  }
}
