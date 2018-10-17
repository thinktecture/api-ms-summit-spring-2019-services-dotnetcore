import { ChangeDetectorRef, Component, ElementRef, NgZone, OnDestroy, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { forkJoin } from 'rxjs/observable/forkJoin';
import { map, takeUntil, tap } from 'rxjs/operators';
import { ItemAdded, ItemDeleted, ItemDoneChanged, ItemNameChanged } from '../../../realtime/models/realtime-models';
import { RealtimeService } from '../../../realtime/services/realtime.service';
import { TodoItem } from '../../models/todo-item';
import { TodoList } from '../../models/todo-list';
import { ItemsService } from '../../services/items.service';

@Component({
  selector: 'app-items',
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.scss'],
})
export class ItemsComponent implements OnInit, OnDestroy {
  formGroup: FormGroup;
  showSpinner: boolean;
  activeIndex: number;
  listName: string;

  @ViewChildren('text')
  textInputs: QueryList<ElementRef>;

  private oldItemsMap: { [key: number]: string } = {};
  private enableBlurRefresh = true;
  private listId: number;
  private stop$ = new Subject<void>();

  constructor(
    private readonly itemsService: ItemsService,
    private readonly formBuilder: FormBuilder,
    private readonly zone: NgZone,
    private readonly route: ActivatedRoute,
    private readonly realtimeService: RealtimeService,
    private readonly changeDetector: ChangeDetectorRef,
  ) {
    this.createForm();
  }

  get items(): FormArray {
    return this.formGroup.get('items') as FormArray;
  }

  ngOnInit() {
    this.subscribeToRealtimeEvents();

    this.route.params.pipe(
      map(params => +params['id']),
    ).subscribe(id => {
      this.listId = id;
      this.refresh();
    });
  }

  create() {
    this.items.insert(0, this.createFormEntry());

    this.changeDetector.detectChanges();
    if (this.textInputs && this.textInputs.first) {
      this.textInputs.first.nativeElement.focus();
    }
  }

  submit() {
    const observables: Observable<any>[] = [];

    this.items.controls.forEach(control => {
      const item = control.value;
      if (!item.id && item.text) {
        observables.push(this.itemsService.createItem(this.listId, item.text)
          .pipe(tap(newId => control.patchValue({ id: newId }))));
      }

      if (item.id && item.text && this.oldItemsMap[item.id] !== item.text) {
        observables.push(this.itemsService.updateItem(this.listId, item));
      }
    });

    if (observables.length) {
      this.showSpinner = true;
      forkJoin(observables)
        .subscribe(() => this.refresh());
    }
  }

  onFocus(index: number) {
    this.activeIndex = index;
  }

  onBlur() {
    if (typeof this.activeIndex === 'undefined') {
      return;
    }

    this.activeIndex = void 0;

    if (this.enableBlurRefresh) {
      this.refresh();
    }
  }

  deleteItem(id: number) {
    this.itemsService.removeItem(this.listId, id)
      .subscribe(() => this.onBlur());
  }

  ngOnDestroy(): void {
    this.stop$.next();
  }

  doneClicked(id: number) {
    this.itemsService.toggleItem(this.listId, id)
      .subscribe();
  }

  private hasItem(id: number): boolean {
    return !!this.items.controls.find(p => p.value.id === id);
  }

  private subscribeToRealtimeEvents(): void {
    this.realtimeService.itemAdded$.pipe(takeUntil(this.stop$))
      .subscribe((data: ItemAdded) => !this.hasItem(data.itemId) && this.items.push(this.createFormEntry({
        id: data.itemId,
        text: data.itemName,
      })));

    this.realtimeService.itemDeleted$.pipe(takeUntil(this.stop$))
      .subscribe((data: ItemDeleted) => this.hasItem(data.itemId) &&
        this.items.removeAt(this.items.controls.findIndex(p => p.value.id === data.itemId)));

    this.realtimeService.itemNameChanged$.pipe(takeUntil(this.stop$))
      .subscribe((data: ItemNameChanged) => this.hasItem(data.itemId) &&
        this.items.controls.find(p => p.value.id === data.itemId).patchValue({
          text: data.newName,
        }));

    this.realtimeService.itemDoneChanged$.pipe(takeUntil(this.stop$))
      .subscribe((data: ItemDoneChanged) => this.hasItem(data.itemId) &&
        this.items.controls.find(p => p.value.id === data.itemId).patchValue({
          done: data.done,
        }));
  }

  private refresh() {
    this.showSpinner = true;
    this.enableBlurRefresh = false;

    this.itemsService.list(this.listId)
      .subscribe(this.processServerList.bind(this));
  }

  private processServerList(list: TodoList) {
    while (this.items.length) {
      this.items.removeAt(0);
    }

    this.listName = list.name;
    list.items.forEach(list => this.items.push(this.createFormEntry(list)));

    this.oldItemsMap = (this.formGroup.value.items as TodoItem[]).reduce((map, current) => {
      map[current.id] = current.text;
      return map;
    }, {});

    this.showSpinner = false;

    this.zone.runOutsideAngular(() => setTimeout(() => this.enableBlurRefresh = true, 250));
  }

  private createFormEntry(item?: TodoItem) {
    return this.formBuilder.group({
      id: item && item.id,
      text: item && item.text,
      done: item && item.done,
    });
  }

  private createForm() {
    this.formGroup = this.formBuilder.group({
      items: this.formBuilder.array([]),
    });
  }
}
