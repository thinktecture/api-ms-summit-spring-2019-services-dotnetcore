import { Directive, ElementRef, EventEmitter, HostListener, Output } from '@angular/core';

@Directive({
  selector: '[outsideClick]',
})
export class OutsideClickDirective {
  @Output()
  outsideClick: EventEmitter<void> = new EventEmitter<void>();

  constructor(private _elementRef: ElementRef) {
  }

  @HostListener('document:click', ['$event.target'])
  onClick(targetElement): void {
    this._onClick(targetElement);
  }

  @HostListener('document:touchstart', ['$event.target'])
  onTouch(targetElement): void {
    this._onClick(targetElement);
  }

  private _onClick(targetElement): void {
    const clickedInside = this._elementRef.nativeElement.contains(targetElement);

    if (!clickedInside) {
      this.outsideClick.emit(null);
    }
  }
}
