export interface ListAdded {
  listId: number;
  listName: string;
}

export interface ListRenamed {
  listId: number;
  newName: string,
}

export interface ListDeleted {
  listId: number;
}

export interface ItemAdded {
  listId: number;
  itemId: number;
  itemName: string;
}

export interface ItemNameChanged {
  listId: number;
  itemId: number;
  newName: string;
}

export interface ItemDoneChanged {
  listId: number;
  itemId: number;
  done: boolean;
}

export interface ItemDeleted {
  listId: number;
  itemId: number;
}

