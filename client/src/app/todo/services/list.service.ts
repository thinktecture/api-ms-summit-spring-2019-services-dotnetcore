import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TodoList } from '../models/todo-list';
import { TodoModuleConfiguration } from '../models/todo-module-configuration';

@Injectable()
export class ListService {
  constructor(private readonly http: HttpClient, private readonly configuration: TodoModuleConfiguration) {
  }

  list(): Observable<TodoList[]> {
    return this.http.get<TodoList[]>(`${this.configuration.apiUrl}/list`).pipe(
      map(data => Object.keys(data).map(key => ({ id: +key, name: data[key] } as TodoList))),
    );
  }

  createList(name: string): Observable<number> {
    return this.http.post<{id: number}>(`${this.configuration.apiUrl}/list`, { value: name }).pipe(
      map(result => result.id),
    );
  }

  updateList(list: TodoList): Observable<void> {
    return this.http.put(`${this.configuration.apiUrl}/list/${list.id}`, { value: list.name })
      .pipe(map(() => void 0));
  }

  removeList(id: number): Observable<void> {
    return this.http.delete(`${this.configuration.apiUrl}/list/${id}`)
      .pipe(map(() => void 0));
  }
}
