import Vue from 'vue';

export const alertDialog = Vue.observable<AlertOptions>({
  text: 'Please confirm',
  show: false,
});

export interface AlertOptions {
  cb?: (accept: boolean) => Promise<void>;
  width?: string;
  ok?: string;
  title?: string;
  text: string;
  cancel?: string | boolean;
  show?: boolean;
}

export interface useAlertResult {
  show(opt: AlertOptions);
}

export function useAlert(): useAlertResult {
  function show(opt: AlertOptions) {
    alertDialog.show = false;
    opt.show = true;
    Object.assign(alertDialog, opt);
  }

  return {
    show,
  };
}
