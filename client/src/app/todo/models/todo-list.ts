import { TodoItem } from './todo-item';

export interface TodoList {
  id: number;
  name: string;

  items?: TodoItem[];
}
