import { ChangeDetectorRef, Component, ElementRef, NgZone, OnDestroy, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { forkJoin } from 'rxjs/observable/forkJoin';
import { takeUntil, tap } from 'rxjs/operators';
import { ListAdded, ListDeleted, ListRenamed } from '../../../realtime/models/realtime-models';
import { RealtimeService } from '../../../realtime/services/realtime.service';
import { TodoList } from '../../models/todo-list';
import { ListService } from '../../services/list.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
})
export class ListComponent implements OnInit, OnDestroy {
  formGroup: FormGroup;
  showSpinner: boolean;
  activeIndex: number;

  @ViewChildren('text')
  textInputs: QueryList<ElementRef>;

  private oldItemsMap: { [key: number]: string } = {};
  private enableBlurRefresh = true;
  private stop$ = new Subject<void>();

  constructor(
    private readonly listService: ListService,
    private readonly formBuilder: FormBuilder,
    private readonly zone: NgZone,
    private readonly realtimeService: RealtimeService,
    private readonly changeDetector: ChangeDetectorRef,
  ) {
    this.createForm();
  }

  get lists(): FormArray {
    return this.formGroup.get('lists') as FormArray;
  }

  ngOnInit() {
    this.subscribeToRealtimeEvents();
    this.refresh();
  }

  create() {
    this.lists.insert(0, this.createFormEntry());

    this.changeDetector.detectChanges();
    if (this.textInputs && this.textInputs.first) {
      this.textInputs.first.nativeElement.focus();
    }
  }

  submit() {
    const observables: Observable<any>[] = [];

    this.lists.controls.forEach(control => {
      const list = control.value;

      if (!list.id && list.name) {
        observables.push(this.listService.createList(list.name)
          .pipe(tap(newId => control.patchValue({ id: newId }))));
      }

      if (list.id && list.name && this.oldItemsMap[list.id] !== list.name) {
        observables.push(this.listService.updateList(list));
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
    this.listService.removeList(id)
      .subscribe(() => this.onBlur());
  }

  ngOnDestroy(): void {
    this.stop$.next();
  }

  private hasList(id: number): boolean {
    return !!this.lists.controls.find(p => p.value.id === id);
  }

  private subscribeToRealtimeEvents(): void {
    this.realtimeService.listAdded$.pipe(takeUntil(this.stop$))
      .subscribe((data: ListAdded) => !this.hasList(data.listId) && this.lists.push(this.createFormEntry({
        id: data.listId,
        name: data.listName,
      })));

    this.realtimeService.listDeleted$.pipe(takeUntil(this.stop$))
      .subscribe((data: ListDeleted) => this.hasList(data.listId) &&
        this.lists.removeAt(this.lists.controls.findIndex(p => p.value.id === data.listId)));

    this.realtimeService.listRenamed$.pipe(takeUntil(this.stop$))
      .subscribe((data: ListRenamed) => this.hasList(data.listId) && this.lists.controls.find(p => p.value.id === data.listId).patchValue({
        name: data.newName,
      }));
  }

  private refresh() {
    this.showSpinner = true;
    this.enableBlurRefresh = false;

    this.listService.list()
      .subscribe(this.processServerList.bind(this));
  }

  private processServerList(lists: TodoList[]) {
    while (this.lists.length) {
      this.lists.removeAt(0);
    }

    lists.forEach(list => this.lists.push(this.createFormEntry(list)));

    this.oldItemsMap = (this.formGroup.value.lists as TodoList[]).reduce((map, current) => {
      map[current.id] = current.name;
      return map;
    }, {});

    this.showSpinner = false;

    this.zone.runOutsideAngular(() => setTimeout(() => this.enableBlurRefresh = true, 250));
  }

  private createFormEntry(list?: TodoList) {
    return this.formBuilder.group({
      id: list && list.id,
      name: list && list.name,
    });
  }

  private createForm() {
    this.formGroup = this.formBuilder.group({
      lists: this.formBuilder.array([]),
    });
  }
}
