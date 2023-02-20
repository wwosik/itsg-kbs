import { Injectable } from '@angular/core';

const defaultTexts = {
	common: {
		buttons: {
			cancel: 'Abbrechen',
		},
	},
};

@Injectable({
	providedIn: 'root',
})
export class TextsService {
	public texts = defaultTexts;

	constructor() {}
}
