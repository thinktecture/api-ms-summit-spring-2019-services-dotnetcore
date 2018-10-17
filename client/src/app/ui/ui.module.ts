import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CardComponent } from './components/card/card.component';
import { OutsideClickDirective } from './directives/outside-click.directive';

@NgModule({
  declarations: [CardComponent, OutsideClickDirective],
  exports: [CardComponent, OutsideClickDirective],
  imports: [CommonModule],
})
export class UiModule {

}
